using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSLColorLearner
{
    public class PLYFileException : Exception
    {
        public PLYFileException(string message) : base(message) { }
    }

    public class PLYFileHeaderFormatException : PLYFileException
    {
        public PLYFileHeaderFormatException(string message) : base(message) { }
    }

    public class PLYFileVertexLineFormatException : PLYFileException
    {
        PLYFileVertexLineFormatException(string message) : base(message) { }
    }

    /// <summary>
    /// A vertex to be saved into the PLY file.
    /// </summary>
    public class PLYFileVertex
    {
        public PLYFileVertex()
        {
            PixelColor = new Color();
        }

        public int VertexIndex { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public Color PixelColor { get; set; }
    }

    public class PLYFileFace
    {
        public PLYFileFace()
        {
            FaceVertices = new List<PLYFileVertex>();
        }

        public List<PLYFileVertex> FaceVertices { get; set; }
    }

    public class PLYHeader
    {
        public PLYHeader()
        {
            ElementVertexProperties = new List<PLYHeaderProperty>();
            ElementFaceProperties = new List<PLYHeaderProperty>();
        }

        public int ElementVertexCount { get; set; }
        public int ElementVertexStartLine { get; set; }
        public List<PLYHeaderProperty> ElementVertexProperties { get; set; }
        public int ElementFaceCount { get; set; }
        public int ElementFaceStartLine { get; set; }
        public List<PLYHeaderProperty> ElementFaceProperties { get; set; }
        public int HeaderLength { get; set; }
    }

    public class PLYHeaderProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class PLYFile
    {
        public PLYFile()
        {
            Vertices = new List<PLYFileVertex>();
            Faces = new List<PLYFileFace>();
        }

        enum CurrentHeaderSectionBeingParsed
        {
            NA, Vertex, Face
        }

        public List<PLYFileVertex> Vertices { get; set; }
        public List<PLYFileFace> Faces { get; set; }

        public PLYHeader Header { get; set; }

        public static PLYFile ReadPLYFile(string filePath)
        {
            PLYFile file = new PLYFile();
            List<PLYFileVertex> vertices = new List<PLYFileVertex>();

            // read all the lines from the file. 
            IEnumerable<string> lines = File.ReadAllLines(filePath);

            // parse the header out of those lines. 
            file.Header = ParseHeader(lines);

            // parse the body
            ParseBody(file, lines);

            return file;
        }

        private static void ParseVertexPortionOfBody(PLYFile file, string[] lines)
        {
            string[] vertexLines = new string[file.Header.ElementVertexCount];
            Array.Copy(lines, file.Header.ElementVertexStartLine, vertexLines, 0, vertexLines.Length);

            //foreach (string line in vertexLines)
            for (int i = 0; i < vertexLines.Length; i++)
            {
                string line = vertexLines[i];

                string[] bits = line.Split(' ');
                PLYFileVertex vertex = new PLYFileVertex();

                int red = 0, blue = 0, green = 0;
                for (int c = 0; c < file.Header.ElementVertexProperties.Count; c++)
                {
                    PLYHeaderProperty prop = file.Header.ElementVertexProperties[c];

                    switch (prop.Name)
                    {
                        case "x":
                            vertex.X = double.Parse(bits[c]);
                            break;
                        case "y":
                            vertex.Y = double.Parse(bits[c]);
                            break;
                        case "z":
                            vertex.Z = double.Parse(bits[c]);
                            break;
                        case "red":
                            red = int.Parse(bits[c]);
                            break;
                        case "green":
                            green = int.Parse(bits[c]);
                            break;
                        case "blue":
                            blue = int.Parse(bits[c]);
                            break;
                    }
                }

                vertex.VertexIndex = i;
                vertex.PixelColor = Color.FromArgb(red, green, blue);
                file.Vertices.Add(vertex);
            }
        }

        private static void ParseFacePortionOfBody(PLYFile file, string[] lines)
        {
            string[] faceLines = new string[file.Header.ElementFaceCount];
            Array.Copy(lines, file.Header.ElementFaceStartLine, faceLines, 0, faceLines.Length);

            foreach (string line in faceLines)
            {
                PLYFileFace face = new PLYFileFace();
                string[] bits = line.Split(' ');

                int numberOfVerticies = int.Parse(bits[0]);

                for (int c = 0; c < numberOfVerticies; c++)
                {
                    int vertexIndex = int.Parse(bits[c + 1]);

                    face.FaceVertices.Add(file.Vertices[vertexIndex]);
                }

                file.Faces.Add(face);
            }
        }

        private static void ParseBody(PLYFile file, IEnumerable<string> lines)
        {
            string[] linesAsArray = lines.ToArray();

            ParseVertexPortionOfBody(file, linesAsArray);
            ParseFacePortionOfBody(file, linesAsArray);

            return;
        }

        private static PLYHeader ParseHeader(IEnumerable<string> lines)
        {
            PLYHeader header = new PLYHeader();

            // identify all the lines that make up the header. 
            List<string> headerLines = new List<string>();
            foreach (string line in lines)
            {
                headerLines.Add(line.ToLower());
                if (line == "end_header")
                {
                    break;
                }
            }

            // go through each of the element identifiers and
            // process each accordingly. the assumption here is the order
            // in which the header elements appear in the header is the order
            // in which they occur in the body. 
            header.HeaderLength = headerLines.Count;
            int zeroIndexStartLineNumber = header.HeaderLength;

            CurrentHeaderSectionBeingParsed sectionBeingParsed = CurrentHeaderSectionBeingParsed.NA;
            for (int c = 0; c < headerLines.Count; c++)
            {
                string headerLine = headerLines[c];

                // determine if we're looking at a new element...
                if (headerLine.StartsWith("element "))
                {
                    string[] bits = headerLine.Split(' ');
                    switch (bits[1])
                    {
                        case "vertex":
                            header.ElementVertexCount = int.Parse(bits[2]);
                            header.ElementVertexStartLine = zeroIndexStartLineNumber;
                            zeroIndexStartLineNumber += header.ElementVertexCount;

                            sectionBeingParsed = CurrentHeaderSectionBeingParsed.Vertex;

                            break;
                        case "face":
                            header.ElementFaceCount = int.Parse(bits[2]);
                            header.ElementFaceStartLine = zeroIndexStartLineNumber;
                            zeroIndexStartLineNumber += header.ElementFaceCount;

                            sectionBeingParsed = CurrentHeaderSectionBeingParsed.Face;

                            break;
                    }
                }
                // otherwise see if we're looking at a property for an element we're parsing....
                else if (headerLine.StartsWith("property "))
                {
                    string[] bits = headerLine.Split(' ');

                    // identify the element we're parsing so we assign this property to the proper element. 
                    switch (sectionBeingParsed)
                    {
                        case CurrentHeaderSectionBeingParsed.Vertex:
                            header.ElementVertexProperties.Add(new PLYHeaderProperty { Name = bits[2], Value = bits[1] });
                            break;
                        case CurrentHeaderSectionBeingParsed.Face:
                            header.ElementFaceProperties.Add(new PLYHeaderProperty { Name = bits[4], Value = headerLine });
                            break;
                    }
                }
            }

            return header;
        }

        //private static PLYFileVertex ProcessVertexLine(string line, int numberOfExpectedValues, int xIndex, int yIndex, int zIndex, int redIndex, int greenIndex, int blueIndex)
        //{
        //    string[] vertexLine = line.Split(' ');

        //    if (vertexLine.Length != numberOfExpectedValues)
        //    {
        //        throw new Exception("Improperly formatted vertex line.");
        //    }

        //    PLYFileVertex point = new PLYFileVertex();

        //    point.X = double.Parse(vertexLine[xIndex]);
        //    point.Y = double.Parse(vertexLine[yIndex]);
        //    point.Z = double.Parse(vertexLine[zIndex]);

        //    int red = int.Parse(vertexLine[redIndex]);
        //    int green = int.Parse(vertexLine[greenIndex]);
        //    int blue = int.Parse(vertexLine[blueIndex]);

        //    point.PixelColor = Color.FromArgb(red, green, blue);

        //    //vertices.Add(point);

        //    //currentVertexCount++;
        //}

        /// <summary>
        /// Saves the file to disk and ends the recording session.
        /// </summary>
        public void Save(string filePath)
        {
            if (!filePath.EndsWith(".ply"))
            {
                filePath += ".ply";
            }

            using (FileStream fs = File.Create(filePath))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("ply");
                    sw.WriteLine("format ascii 1.0");
                    sw.WriteLine("element vertex " + Vertices.Count.ToString());
                    sw.WriteLine("property float x");
                    sw.WriteLine("property float y");
                    sw.WriteLine("property float z");
                    sw.WriteLine("property uchar red");
                    sw.WriteLine("property uchar green");
                    sw.WriteLine("property uchar blue");
                    sw.WriteLine("element face " + Faces.Count.ToString());
                    sw.WriteLine("property list uchar uint vertex_indices");
                    sw.WriteLine("end_header");

                    foreach (PLYFileVertex vertex in Vertices)
                    {
                        sw.WriteLine("{0} {1} {2} {3} {4} {5}", vertex.X, vertex.Y, vertex.Z,
                            vertex.PixelColor.R, vertex.PixelColor.G, vertex.PixelColor.B);
                    }

                    foreach (PLYFileFace face in Faces)
                    {
                        int numberOfVertices = face.FaceVertices.Count;
                        sw.WriteLine(numberOfVertices.ToString() + " " + string.Join(" ", face.FaceVertices.Select(m => m.VertexIndex.ToString()).ToArray()));
                    }
                }
            }
        }
    }
}