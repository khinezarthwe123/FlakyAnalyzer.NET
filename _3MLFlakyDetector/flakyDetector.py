import argparse
from predict.predict_flaky import predict_flaky
from training.train_model import training_model_randomforest

parser = argparse.ArgumentParser(description="Train or predict flaky tests from a given CSV file.")
parser.add_argument("file", type=str, help="Path to the input CSV file")
parser.add_argument("--mode", choices=["train", "predict"], default="predict", help="Run mode: train or predict")
args = parser.parse_args()

if args.mode == "train":
    training_model_randomforest(args.file)
else:
    predict_flaky(args.file)


# mode = "predict"
# if mode == "train":
#     training_model_randomforest(r"C:\Users\khine\Desktop\FlakyAnalyzer.NET\_2TestSuiteLogger\TestExecutionLog.csv")
# else:
#     predict_flaky(r"C:\Users\khine\Desktop\FlakyAnalyzer.NET\_2TestSuiteLogger\NewTestData.csv")