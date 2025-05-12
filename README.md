# FlakyAnalyzer.Net

# How to Use This Flaky Test Detection Tool
This tool helps detect flaky tests in .NET Selenium projects using test execution logs and a lightweight machine learning model.  
It includes two main components:  
Logging Component (C# + NUnit)  
Prediction Component (Python + scikit-learn)  

# 1. Run the Test Suite and Collect Logs
Wrap your NUnit test cases in the provided ExecuteWithRetry() method to enable retry tracking.  
Use the included C# logging module to record test metadata:  
Test name  
Status (Pass/Fail)  
Duration  
Retry count  
Timestamp  
Browser name  
Browser version  
After test execution, a .csv log file will be created (e.g., TestExecutionLog.csv).  

# 2. Train the ML Model (Python)
To train a flaky test prediction model:  
python flakyDetector.py your_log_file.csv --mode train  

This will:  
Preprocess the log file  
Add duration-based features (mean, std)  
Train a Random Forest classifier  
Save the preprocessor and model as .pkl files  
Generate reports   

Output:  
model/flaky_model.pkl  
model/preprocessor.pkl  
TestExecutionLog_train_eval.csv – Predictions + actual labels  
classification_report.txt – Precision, recall, F1 summary  
confusion_matrix.png – Visual breakdown of prediction results  
feature_importance.png – Top features ranked by importance  
flaky_distribution.png – Flaky vs stable test counts  
flaky_by_testname.png – Flaky test predictions by test case  
flaky_by_browser.png – Flaky test predictions by browser  
false_positive_TestName.png – Stable tests wrongly flagged as flaky  

# 3. Run Flakiness Prediction on New Logs (Python)
To make predictions on a new log file (after training):  
python flakyDetector.py your_log_file.csv --mode predict  

This will:  
Load the saved model and preprocessor  
Predict flaky tests based on the new log data  
Add a Predicted_Flaky column to the CSV  
Generate reports  

Output:  
predicted.csv – Prediction results on new test data  
flaky_distribution.png – Flaky vs stable prediction breakdown  
flaky_by_testname.png – Flaky test predictions grouped by test case  
flaky_by_browser.png – Flaky predictions grouped by browser environment  

# 4. Folder Structure
├── ASP.Net MVC/               # ASP.NET MVC application (for logging integration)  
├── TestSuite/                 # Selenium + NUnit test cases with retry logic  
│   ├── Logger/                # C# logging helper module (integrates with NUnit)  
│   ├── sample_logs/           # Example CSV logs for testing  
├── MLModule/                  # Python scripts for ML model training and prediction  
│   ├── train_model.py         # Script to train the model from log data  
│   ├── predict_flaky.py       # Script to predict flakiness on new logs  
│   ├── visualize/             # Functions for generating evaluation plots  
│   ├── model/                 # Saved ML model and preprocessor (.pkl files)  
│   ├── plot/                  # Output plots from training and prediction results  
└── README.md                  # Project instructions and usage documentation  


# 5. Requirements
.NET 6 or later (for test suite and logger)  
Python 3.8+  
Install dependencies:  
pip install -r requirements.txt  

# Notes
The model uses dynamic features (test duration, retries, browser info).  
Timing-based features (DurationMean, DurationStdDev) significantly improve flaky test detection.  
Designed to work with any .NET Selenium project using NUnit.  
