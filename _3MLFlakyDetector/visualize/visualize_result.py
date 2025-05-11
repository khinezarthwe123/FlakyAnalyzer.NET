import os
import pandas as pd
import matplotlib.pyplot as plt
from sklearn.metrics import classification_report
from sklearn.metrics import confusion_matrix, ConfusionMatrixDisplay

def plot_confusion_matrix(df, file_path):
    if "Flaky" not in df.columns or "Predicted_Flaky" not in df.columns:
        print("Confusion matrix is only for Training/Testing.")
        return
   
    cm = confusion_matrix(df["Flaky"], df["Predicted_Flaky"], labels=[0, 1])
    disp = ConfusionMatrixDisplay(confusion_matrix=cm, display_labels=["Not Flaky", "Flaky"])
    disp.plot(cmap="Blues")
    plt.title("Confusion Matrix")
    plt.tight_layout()
    
    output_path = build_plot_path(file_path, "confusion_matrix.png")
    plt.savefig(output_path)
    plt.close()

def plot_flaky_distribution(df, file_path):
    flaky_counts = df["Predicted_Flaky"].value_counts().reindex([0, 1], fill_value=0)
    flaky_counts.index = ["Not Flaky", "Flaky"]
    flaky_counts.plot(kind="bar", color=["skyblue", "tomato"])
    plt.title("Predicted Flaky vs Non-Flaky Test Cases")
    plt.ylabel("Count")
    plt.xticks(rotation=0)
    plt.tight_layout()
    output_path = build_plot_path(file_path, "flaky_distribution.png")
    plt.savefig(output_path)
    plt.close()

def plot_flaky_by_browser(df, file_path):
    if "Predicted_Flaky" not in df.columns or "BrowserName" not in df.columns:
        return
    
    grouped = df.groupby(["BrowserName", "Predicted_Flaky"]).size().unstack(fill_value=0)
    grouped = grouped.reindex(columns=[0, 1], fill_value=0)
    grouped.columns = ["Not Flaky", "Flaky"]
    grouped.plot(kind="bar", stacked=True, color=["skyblue", "tomato"])
    plt.title("Flaky Predictions by Browser")
    plt.ylabel("Number of Test Cases")
    plt.xticks(rotation=45)
    plt.tight_layout()
    output_path = build_plot_path(file_path, "_flaky_by_browser.png")
    plt.savefig(output_path)
    plt.close()

def plot_flaky_by_testname(df, file_path):
    if "Predicted_Flaky" not in df.columns or "TestName" not in df.columns:
        return
    
    grouped = df.groupby(["TestName", "Predicted_Flaky"]).size().unstack(fill_value=0)
    grouped = grouped.reindex(columns=[0, 1], fill_value=0)
    grouped.columns = ["Not Flaky", "Flaky"]
    grouped = grouped.sort_values(by="Flaky", ascending=False)
    grouped.plot(kind="bar", stacked=True, figsize=(10, 6), color=["skyblue", "tomato"])
    plt.title("Flaky Predictions by Test Name")
    plt.ylabel("Number of Test Executions")
    plt.xticks(rotation=90)
    plt.tight_layout()
    output_path = build_plot_path(file_path, "_flaky_by_testname.png")
    plt.savefig(output_path)
    plt.close()

def save_classification_report(df, file_path):
    if "Flaky" not in df.columns or "Predicted_Flaky" not in df.columns:
        return
    
    report = classification_report(df["Flaky"], df["Predicted_Flaky"], target_names=["Not Flaky", "Flaky"])
    output_path = build_plot_path(file_path, "_classification_report.txt")

    with open(output_path, "w") as f:
        f.write("Classification Report\n")
        f.write("=====================\n")
        f.write(report)

def plot_feature_importance(model, df, feature_names, file_path):
    
    # After training
    importances = model.feature_importances_    

    # Combine into a DataFrame for easy viewing
    feature_importance_df = pd.DataFrame({
        'Feature': feature_names,
        'Importance': importances
    }).sort_values(by='Importance', ascending=False)
    plt.figure(figsize=(10, 6))
    feature_importance_df.plot(
        kind='barh', x='Feature', y='Importance', legend=False, color='skyblue')
    plt.title('Important Features')
    plt.xlabel('Importance Score')
    plt.gca().invert_yaxis()
    plt.tight_layout()
    plt.grid(axis='x', linestyle='--', alpha=0.7)
    plt.tight_layout()
    output_path = build_plot_path(file_path, "_feature_importance.png")
    plt.savefig(output_path)
    plt.close()

def plot_false_positive_TestName(df, file_path):
    if "Flaky" not in df.columns or "Predicted_Flaky" not in df.columns:
        return  # Required columns are missing
    
    false_positives = df[(df['Flaky'] == 0) & (df['Predicted_Flaky'] == 1)]
    fp_counts = false_positives['TestName'].value_counts()

    if not fp_counts.empty:
        plt.figure(figsize=(10, 6))
        fp_counts.plot(kind='bar', color='orangered')
        plt.title('False Positive Predictions by Test Name')
        plt.xlabel('Test Name')
        plt.ylabel('Number of False Positives')
        plt.xticks(rotation=45, ha='right')
        plt.tight_layout()
        plt.grid(axis='y', linestyle='--', alpha=0.7)
        output_path = build_plot_path(file_path, "_false_positive_TestName")
        plt.savefig(output_path)
        plt.close()

def visualize_model_evaluation(df, file_path):    
    plot_confusion_matrix(df, file_path)
    plot_flaky_distribution(df, file_path)
    plot_flaky_by_browser(df, file_path)
    plot_flaky_by_testname(df, file_path)
    save_classification_report(df, file_path)
    plot_false_positive_TestName(df, file_path)    
    print("Visualizations saved.")

def build_plot_path(file_path, suffix):
    os.makedirs("plot", exist_ok=True)
    filename = os.path.basename(file_path).replace(".csv", f"_{suffix}")
    return os.path.join("plot", filename)
