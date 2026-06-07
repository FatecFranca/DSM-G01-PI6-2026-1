import pandas as pd
import joblib

from sklearn.model_selection import train_test_split, cross_val_score
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import accuracy_score, classification_report, confusion_matrix
from sklearn.preprocessing import LabelEncoder
from xgboost import XGBClassifier


DATASET_SLEEP_PATH = "Sleep_health_and_lifestyle_dataset.csv"
DATASET_MOBILE_PATH = "sleep_mobile_stress_dataset_15000.csv"

RF_MODEL_PATH = "rf_combined_sleep_model.pkl"
"""XGB_MODEL_PATH = "xgb_combined_sleep_model.pkl"""
"""LABEL_ENCODER_PATH = "label_encoder_combined.pkl"""

MAX_MOBILE_ROWS = 500


def load_datasets():
    df_sleep = pd.read_csv(DATASET_SLEEP_PATH)
    df_mobile = pd.read_csv(DATASET_MOBILE_PATH)
    return df_sleep, df_mobile


def normalize_columns(df):
    df = df.copy()
    df.columns = (
        df.columns
        .str.strip()
        .str.lower()
        .str.replace(" ", "_")
    )
    return df


def prepare_sleep_dataset(df):
    df = normalize_columns(df)

    df = df.drop(columns=["person_id"], errors="ignore")

    df["sleep_disorder"] = df["sleep_disorder"].fillna("No Disorder")

    df["bmi_category"] = df["bmi_category"].replace({
        "Normal": "Normal Weight"
    })

    bp_split = df["blood_pressure"].str.split("/", expand=True)

    df["bp_systolic"] = pd.to_numeric(bp_split[0], errors="coerce")
    df["bp_diastolic"] = pd.to_numeric(bp_split[1], errors="coerce")

    df["bp_mean"] = (
        df["bp_systolic"] + (2 * df["bp_diastolic"])
    ) / 3

    df = df.drop(columns=["blood_pressure", "bp_systolic", "bp_diastolic"], errors="ignore")

    df["sleep_duration_hours"] = df["sleep_duration"]
    df["physical_activity_minutes"] = df["physical_activity_level"]

    df["used_phone_before_sleep"] = 0
    df["consumed_caffeine"] = 0
    df["consumed_alcohol"] = 0
    df["mental_fatigue_score"] = df["stress_level"]

    selected_cols = [
        "age",
        "gender",
        "occupation",
        "sleep_duration_hours",
        "quality_of_sleep",
        "stress_level",
        "physical_activity_minutes",
        "bmi_category",
        "heart_rate",
        "daily_steps",
        "bp_mean",
        "used_phone_before_sleep",
        "consumed_caffeine",
        "consumed_alcohol",
        "mental_fatigue_score",
        "sleep_disorder"
    ]

    return df[selected_cols]


def create_pseudo_label(row):
    sleep_quality = row["sleep_quality_score"]
    sleep_duration = row["sleep_duration_hours"]
    stress = row["stress_level"]
    fatigue = row["mental_fatigue_score"]

    if sleep_quality <= 4 and sleep_duration < 6.5 and stress >= 7:
        return "Insomnia"

    if sleep_quality <= 5 and fatigue >= 7 and stress >= 7:
        return "Insomnia"

    return "No Disorder"


def prepare_mobile_dataset(df):
    df = normalize_columns(df)

    df = df.sample(
        n=min(MAX_MOBILE_ROWS, len(df)),
        random_state=42
    )

    df["used_phone_before_sleep"] = (
        df["phone_usage_before_sleep_minutes"] > 0
    ).astype(int)

    df["consumed_caffeine"] = (
        df["caffeine_intake_cups"] > 0
    ).astype(int)

    df["consumed_alcohol"] = 0

    df["quality_of_sleep"] = df["sleep_quality_score"]

    df["bmi_category"] = "Unknown"
    df["heart_rate"] = pd.NA
    df["daily_steps"] = pd.NA
    df["bp_mean"] = pd.NA

    df["sleep_disorder"] = df.apply(create_pseudo_label, axis=1)

    selected_cols = [
        "age",
        "gender",
        "occupation",
        "sleep_duration_hours",
        "quality_of_sleep",
        "stress_level",
        "physical_activity_minutes",
        "bmi_category",
        "heart_rate",
        "daily_steps",
        "bp_mean",
        "used_phone_before_sleep",
        "consumed_caffeine",
        "consumed_alcohol",
        "mental_fatigue_score",
        "sleep_disorder"
    ]

    return df[selected_cols]


def preprocess_combined_data(df):
    df = df.copy()

    print("\nDistribuição do alvo:")
    print(df["sleep_disorder"].value_counts())

    X = df.drop(columns=["sleep_disorder"])
    y = df["sleep_disorder"]

    numeric_cols = [
        "age",
        "sleep_duration_hours",
        "quality_of_sleep",
        "stress_level",
        "physical_activity_minutes",
        "heart_rate",
        "daily_steps",
        "bp_mean",
        "used_phone_before_sleep",
        "consumed_caffeine",
        "consumed_alcohol",
        "mental_fatigue_score"
    ]

    categorical_cols = [
        "gender",
        "occupation",
        "bmi_category"
    ]

    for col in numeric_cols:
        if col in X.columns:
            X[col] = pd.to_numeric(X[col], errors="coerce")
            X[col] = X[col].fillna(X[col].median())

    for col in categorical_cols:
        if col in X.columns:
            X[col] = X[col].astype("str")
            X[col] = X[col].fillna("Unknown")

    X = pd.get_dummies(
        X,
        columns=categorical_cols,
        drop_first=True
    )

    return X, y

def train_and_compare_models(X, y):
    X_train, X_test, y_train, y_test = train_test_split(
        X,
        y,
        test_size=0.2,
        random_state=42,
        stratify=y
    )

    print("\n========== RANDOM FOREST ==========")

    rf_model = RandomForestClassifier(
        n_estimators=300,
        random_state=42,
        class_weight="balanced"
    )

    rf_model.fit(X_train, y_train)
    rf_pred = rf_model.predict(X_test)

    print("Accuracy:", accuracy_score(y_test, rf_pred))
    print("\nRelatório:")
    print(classification_report(y_test, rf_pred))
    print("\nMatriz de confusão:")
    print(confusion_matrix(y_test, rf_pred))

    rf_scores = cross_val_score(rf_model, X, y, cv=5, scoring="accuracy")
    print("\nCross-validation:")
    print(rf_scores)
    print("Média:", rf_scores.mean())

    print("\n========== XGBOOST ==========")

    le = LabelEncoder()
    y_encoded = le.fit_transform(y)

    X_train_xgb, X_test_xgb, y_train_xgb, y_test_xgb = train_test_split(
        X,
        y_encoded,
        test_size=0.2,
        random_state=42,
        stratify=y_encoded
    )

    xgb_model = XGBClassifier(
        n_estimators=300,
        learning_rate=0.05,
        max_depth=5,
        random_state=42,
        eval_metric="mlogloss"
    )

    xgb_model.fit(X_train_xgb, y_train_xgb)
    xgb_pred = xgb_model.predict(X_test_xgb)

    print("Accuracy:", accuracy_score(y_test_xgb, xgb_pred))
    print("\nRelatório:")
    print(classification_report(
        y_test_xgb,
        xgb_pred,
        target_names=le.classes_
    ))
    print("\nMatriz de confusão:")
    print(confusion_matrix(y_test_xgb, xgb_pred))

    xgb_scores = cross_val_score(
        xgb_model,
        X,
        y_encoded,
        cv=5,
        scoring="accuracy"
    )

    print("\nCross-validation:")
    print(xgb_scores)
    print("Média:", xgb_scores.mean())

    return rf_model, xgb_model, le


def show_feature_importance(model, X):
    importances = pd.Series(model.feature_importances_, index=X.columns)
    importances = importances.sort_values(ascending=False)

    print("\n========== TOP 15 FEATURES RANDOM FOREST ==========")
    print(importances.head(15))


def save_models(rf_model, xgb_model, le):
    joblib.dump(rf_model, RF_MODEL_PATH)
    joblib.dump(xgb_model, XGB_MODEL_PATH)
    joblib.dump(le, LABEL_ENCODER_PATH)

    print(f"\nModelo Random Forest salvo em: {RF_MODEL_PATH}")
    print(f"Modelo XGBoost salvo em: {XGB_MODEL_PATH}")
    print(f"LabelEncoder salvo em: {LABEL_ENCODER_PATH}")


def main():
    df_sleep, df_mobile = load_datasets()

    df_sleep_prepared = prepare_sleep_dataset(df_sleep)
    df_mobile_prepared = prepare_mobile_dataset(df_mobile)

    df_combined = pd.concat(
        [df_sleep_prepared, df_mobile_prepared],
        ignore_index=True
    )

    print("\nShape dataset principal:", df_sleep_prepared.shape)
    print("Shape dataset mobile:", df_mobile_prepared.shape)
    print("Shape combinado:", df_combined.shape)

    X, y = preprocess_combined_data(df_combined)

    print("\nShape de X:", X.shape)
    print("Shape de y:", y.shape)

    rf_model, xgb_model, le = train_and_compare_models(X, y)

    show_feature_importance(rf_model, X)

    save_models(rf_model, xgb_model, le)


if __name__ == "__main__":
    main()