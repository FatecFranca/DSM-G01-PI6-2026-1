import pandas as pd
import joblib

from sklearn.model_selection import train_test_split, cross_val_score
from sklearn.ensemble import RandomForestRegressor
from sklearn.metrics import mean_absolute_error, mean_squared_error, r2_score


DATASET_PATH = "sleep_mobile_stress_dataset_15000.csv"
MODEL_PATH = "sleep_score_model.pkl"


def load_data(path):
    return pd.read_csv(path)


def preprocess_data(df):
    df = df.copy()

    df.columns = (
        df.columns
        .str.strip()
        .str.lower()
        .str.replace(" ", "_")
    )

    print("Colunas encontradas no dataset:")
    print(df.columns.tolist())

    target = "sleep_quality_score"

    if target not in df.columns:
        raise ValueError(
            f"A coluna alvo '{target}' não foi encontrada. "
            "Verifique o nome correto da coluna no dataset."
        )

    df = df.dropna(subset=[target])

    if "phone_usage_before_sleep_minutes" in df.columns:
        df["phone_usage_before_sleep_minutes"] = pd.to_numeric(
            df["phone_usage_before_sleep_minutes"],
            errors="coerce"
        )

        df["used_phone_before_sleep"] = (
            df["phone_usage_before_sleep_minutes"] > 0
        ).astype(int)

    if "caffeine_intake_cups" in df.columns:
        df["consumed_caffeine"] = (
            df["caffeine_intake_cups"] > 0
        ).astype(int)

    cols_to_drop = [
        target,
        "user_id",
        "caffeine_intake_cups",
        "daily_screen_time_hours",
        "notifications_received_per_day"
    ]

    X = df.drop(columns=cols_to_drop, errors="ignore")
    y = df[target]

    numeric_cols = X.select_dtypes(include=["int64", "float64"]).columns
    categorical_cols = X.select_dtypes(include=["str", "bool", "category"]).columns

    for col in numeric_cols:
        X[col] = X[col].fillna(X[col].median())

    for col in categorical_cols:
        if not X[col].mode().empty:
            X[col] = X[col].fillna(X[col].mode()[0])

    X = pd.get_dummies(X, drop_first=True)

    print("\nFeatures usadas no modelo:")
    print(X.columns.tolist())

    return X, y


def train_regression_model(X, y):
    X_train, X_test, y_train, y_test = train_test_split(
        X,
        y,
        test_size=0.2,
        random_state=42
    )

    model = RandomForestRegressor(
        n_estimators=300,
        random_state=10
    )

    model.fit(X_train, y_train)

    y_pred = model.predict(X_test)

    mae = mean_absolute_error(y_test, y_pred)
    rmse = mean_squared_error(y_test, y_pred) ** 0.5
    r2 = r2_score(y_test, y_pred)

    print("\n========== RESULTADOS REGRESSÃO ==========")
    print("MAE:", mae)
    print("RMSE:", rmse)
    print("R²:", r2)

    scores = cross_val_score(model, X, y, cv=5, scoring="r2")

    print("\nCross-validation R²:")
    print(scores)
    print("Média R²:", scores.mean())

    return model


def show_feature_importance(model, X):
    importances = pd.Series(model.feature_importances_, index=X.columns)
    importances = importances.sort_values(ascending=False)

    print("\n========== TOP 10 FEATURES ==========")
    print(importances.head(10))


def save_model(model, path):
    joblib.dump(model, path)
    print(f"\nModelo salvo em: {path}")


def main():
    df = load_data(DATASET_PATH)

    X, y = preprocess_data(df)

    print("\nShape de X:", X.shape)
    print("Shape de y:", y.shape)

    model = train_regression_model(X, y)

    show_feature_importance(model, X)

    save_model(model, MODEL_PATH)


if __name__ == "__main__":
    main()