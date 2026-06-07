import pandas as pd
import joblib

from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import accuracy_score, classification_report, confusion_matrix

from sklearn.model_selection import cross_val_score


from xgboost import XGBClassifier
from sklearn.preprocessing import LabelEncoder

DATASET_PATH = "Sleep_health_and_lifestyle_dataset.csv"
MODEL_PATH = "sleep_model.pkl"


def load_data(path):
    df = pd.read_csv(path)
    return df


def preprocess_data(df):
    df = df.copy()

    df = df.drop(columns=["Person ID"], errors="ignore")

    df.columns = (
    df.columns
    .str.strip()
    .str.lower()
    .str.replace(" ", "_")  
    )


    df["sleep_disorder"] = df["sleep_disorder"].fillna("No Disorder")

    print(df["sleep_disorder"].value_counts())


    df["bmi_category"] = df["bmi_category"].replace({
        "Normal": "Normal Weight"
    })

    bp_split = df["blood_pressure"].str.split("/", expand=True)

    df["bp_systolic"] = pd.to_numeric(bp_split[0], errors="coerce")
    df["bp_diastolic"] = pd.to_numeric(bp_split[1], errors="coerce")

    df = df.drop(columns=["blood_pressure"])

    X = df.drop(columns=["sleep_disorder"])
    y = df["sleep_disorder"]

    X = pd.get_dummies(X, drop_first=True)

    return X, y


def train_and_compare_models(X, y):
    
    # 🔹 XGBoost precisa de y numérico
    le = LabelEncoder()
    y_encoded = le.fit_transform(y)

    X_train, X_test, y_train_rf, y_test_rf = train_test_split(
        X, y, test_size=0.2, random_state=42
    )

    # Para XGBoost
    y_train_xgb = le.transform(y_train_rf)
    y_test_xgb = le.transform(y_test_rf)

    # =========================
    # 🌳 Random Forest
    # =========================
    print("\n========== RANDOM FOREST ==========")

    rf_model = RandomForestClassifier(
        n_estimators=200,
        random_state=42,
        class_weight="balanced"
    )

    rf_model.fit(X_train, y_train_rf)
    rf_pred = rf_model.predict(X_test)

    print("Accuracy:", accuracy_score(y_test_rf, rf_pred))
    print("\nRelatório:\n", classification_report(y_test_rf, rf_pred))
    print("\nMatriz:\n", confusion_matrix(y_test_rf, rf_pred))

    rf_scores = cross_val_score(rf_model, X, y, cv=5)
    print("\nCross-val:", rf_scores)
    print("Média:", rf_scores.mean())

    # =========================
    # 🚀 XGBoost
    # =========================
    print("\n========== XGBOOST ==========")

    xgb_model = XGBClassifier(
        n_estimators=200,
        learning_rate=0.1,
        max_depth=5,
        random_state=42,
        use_label_encoder=False,
        eval_metric="mlogloss"
    )

    xgb_model.fit(X_train, y_train_xgb)
    xgb_pred = xgb_model.predict(X_test)

    print("Accuracy:", accuracy_score(y_test_xgb, xgb_pred))
    print("\nRelatório:\n", classification_report(y_test_xgb, xgb_pred))
    print("\nMatriz:\n", confusion_matrix(y_test_xgb, xgb_pred))

    xgb_scores = cross_val_score(xgb_model, X, y_encoded, cv=5)
    print("\nCross-val:", xgb_scores)
    print("Média:", xgb_scores.mean())

    return rf_model, xgb_model, le


def save_model(model, path):
    joblib.dump(model, path)
    print(f"\nModelo salvo em: {path}")


def main():
    df = load_data(DATASET_PATH)
    X, y = preprocess_data(df)
    rf_model, xgb_model, le = train_and_compare_models(X, y)
    save_model(xgb_model, "xgb_model.pkl")
    save_model(rf_model, "rf_model.pkl")


if __name__ == "__main__":
    main()