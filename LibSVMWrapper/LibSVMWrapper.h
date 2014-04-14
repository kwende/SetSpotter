// LibSVMWrapper.h

#pragma once

#include "svm.h"
#include <msclr\marshal_cppstd.h>

using namespace System;

namespace LibSVMWrapper {

	public ref class Predict
	{
	public:
		Predict(String^ modelPath);
		!Predict(); 
		double Compute(array<double>^ vector);
	private:
		svm_model* _model; 
	};
}
