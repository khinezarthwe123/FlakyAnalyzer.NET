import os
import pandas as pd
import matplotlib.pyplot as plt
import joblib
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
from visualize.visualize_result import visualize_model_evaluation
from visualize.visualize_result import plot_feature_importance
from sklearn.compose import ColumnTransformer
from sklearn.preprocessing import OneHotEncoder, StandardScaler
from sklearn.pipeline import Pipeline

preprocessor_path = "model/preprocessor.pkl"
model_path = "model/flaky_model.pkl"

def is_flaky(status_series):
    return 1 if status_series.nunique() > 1 else 0

def training_model_randomforest(file_path):
    print("\nLoading dataset")
    df = pd.read_csv(file_path)

    print("\nPreprocessing features")
    df["Flaky"] = df.groupby("TestName")["Status"].transform(is_flaky).astype(int)
    df["Duration"] = df["Duration"].fillna(0)
    df["RetryCount"] = df["RetryCount"].fillna(0)
    df["DurationMean"] = df.groupby('TestName')['Duration'].transform('mean').round(2)
    df["DurationStdDev"] = df.groupby('TestName')['Duration'].transform('std').round(2)
    numerical_features = ["Duration", "RetryCount", "DurationMean", "DurationStdDev"]
    categorical_features = ["BrowserName"]
    preprocessor = ColumnTransformer(
        transformers=[
            ("num", StandardScaler(), numerical_features),
            ("cat", OneHotEncoder(), categorical_features)
        ]
    )

    # Fit and transform on training data
    X = df[numerical_features + categorical_features]
    X_processed = preprocessor.fit_transform(X)
    joblib.dump(preprocessor, preprocessor_path)
    feature_names = preprocessor.get_feature_names_out()
    X = pd.DataFrame(X_processed, columns=feature_names)
    y = df["Flaky"]
    print(df["Flaky"].value_counts())

    print("\nTraining and evaluating model")
    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.3, random_state=42, stratify=y)
    model = RandomForestClassifier(class_weight='balanced', n_estimators=100, random_state=42)
    model.fit(X_train, y_train)
    plot_feature_importance(model, df, feature_names, file_path)
    y_pred = model.predict(X_test)

    print("\nClassification Report:\n")
    print(classification_report(y_test, y_pred))

    #Save to same path used in prediction
    joblib.dump(model, model_path)  
    print("\nModel saved to model/flaky_model.pkl")

    # Attach predictions back to test set for evaluation output
    test_df = df.iloc[y_test.index].copy()
    test_df["Predicted_Flaky"] = y_pred
    filename = os.path.basename(file_path).replace(".csv", "_train_eval.csv")
    output_path = os.path.join("plot", filename)
    test_df.to_csv(output_path, index=False)
    visualize_model_evaluation(test_df, file_path)

