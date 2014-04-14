// This is the main DLL file.

#include "stdafx.h"
#include "LibSVMWrapper.h"

using namespace LibSVMWrapper; 

Predict::Predict(String^ modelPath)
{
	msclr::interop::marshal_context context;
	std::string filePathC = context.marshal_as<std::string>(modelPath);

	_model = svm_load_model(filePathC.c_str());
}

Predict::!Predict()
{
	if (_model != NULL)
	{
		svm_model* temp = _model; 
		svm_free_and_destroy_model(&temp);
		_model = NULL; 
	}
}

double Predict::Compute(array<double>^ vector)
{
	svm_node* node = (svm_node*)malloc(sizeof(svm_node) * (vector->Length + 1)); 

	for (int c = 0; c < vector->Length; c++)
	{
		node[c].index = c + 1; 
		node[c].value = vector[c]; 
	}
	node[vector->Length].index = -1; 

	double ret = svm_predict(_model, node); 

	free(node); 

	return ret; 
}