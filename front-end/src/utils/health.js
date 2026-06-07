export function normalizeHeightToMeters(heightValue) {
  const numericHeight = Number(heightValue);

  if (!Number.isFinite(numericHeight) || numericHeight <= 0) {
    return null;
  }

  return numericHeight > 3 ? numericHeight / 100 : numericHeight;
}

export function calculateBmi(weightValue, heightValue) {
  const numericWeight = Number(weightValue);
  const heightInMeters = normalizeHeightToMeters(heightValue);

  if (!Number.isFinite(numericWeight) || numericWeight <= 0 || !heightInMeters) {
    return null;
  }

  const bmi = numericWeight / (heightInMeters * heightInMeters);

  return Number.isFinite(bmi) ? bmi : null;
}

export function getBmiClassification(bmiValue) {
  const bmi = Number(bmiValue);

  if (!Number.isFinite(bmi) || bmi <= 0) {
    return "";
  }

  if (bmi < 18.5) return "Abaixo do peso";
  if (bmi < 25) return "Peso normal";
  if (bmi < 30) return "Sobrepeso";
  if (bmi < 35) return "Obesidade grau I";
  if (bmi < 40) return "Obesidade grau II";
  return "Obesidade grau III";
}

export function formatBmi(bmiValue) {
  if (!Number.isFinite(bmiValue) || bmiValue <= 0) {
    return "Nao informado";
  }

  return bmiValue.toFixed(2);
}
