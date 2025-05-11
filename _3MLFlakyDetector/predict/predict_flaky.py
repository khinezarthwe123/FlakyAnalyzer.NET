import os
import pandas as pd
import joblib
import matplotlib.pyplot as plt
from sklearn.metrics import confusion_matrix, ConfusionMatrixDisplay
import argparse
from training.train_model import training_model_randomforest
from visualize.visualize_result import visualize_model_evaluation

def predict_flaky(file_path):
    print(f"Loading test data from: {file_path}")
    df = pd.read_csv(file_path)

    model = joblib.load("model/flaky_model.pkl")    
    df["DurationMean"] = df.groupby("TestName")["Duration"].transform("mean").round(2)
    df["DurationStdDev"] = df.groupby("TestName")["Duration"].transform("std").round(2)  
    preprocessor = joblib.load("model/preprocessor.pkl")
    X = df[["Duration", "RetryCount", "BrowserName", "DurationMean", "DurationStdDev"]].copy()
    X_processed = preprocessor.transform(X)    
    feature_names = preprocessor.get_feature_names_out()
    X_df = pd.DataFrame(X_processed, columns=feature_names)

    predictions = model.predict(X_df)
    df["Predicted_Flaky"] = predictions

    filename = os.path.basename(file_path).replace(".csv", "_predicted.csv")
    output_path = os.path.join("plot", filename)

    df.to_csv(output_path, index=False)
    print(f"Predictions saved to: {output_path}")

    visualize_model_evaluation(df, file_path)