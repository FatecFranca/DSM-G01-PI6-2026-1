using Sleep.Domain.Entities;
using Sleep.Domain.Enum;

namespace Sleep.Communication.Events;

public record SleepAnalysisRequestMessage(
    int Age,
    decimal SleepDurationHours,
    int QualityOfSleep,
    int StressLevel,
    int MentalFatigueScore,
    int PhysicalActivityMinutes,
    decimal BpMean,
    int HeartRate,
    int DailySteps,
    int UsedPhoneBeforeSleep,
    int ConsumedCaffeine,
    int ConsumedAlcohol,
    string Gender,
    string Occupation,
    string BmiCategory,
    long SleepRecordId)
{
    public static SleepAnalysisRequestMessage FromSleepRecord(SleepRecord record, string gender, string occupation, long sleepRecordId)
    {
        return new SleepAnalysisRequestMessage(
            Age: record.Age,
            SleepDurationHours: record.DurationHours,
            QualityOfSleep: record.QualityOfSleep,
            StressLevel: record.StressLevel,
            MentalFatigueScore: record.MentalFigure,
            PhysicalActivityMinutes: record.PhysicalActivityMinutes,
            BpMean: record.BloodPressure,
            HeartRate: record.HeartRate,
            DailySteps: record.DailySteps,
            UsedPhoneBeforeSleep: record.ScreenBeforeSleep ? 1 : 0,
            ConsumedCaffeine: record.Caffeine ? 1 : 0,
            ConsumedAlcohol: record.Alcohol ? 1 : 0,
            Gender: MapGender(gender),
            Occupation: occupation,
            BmiCategory: MapBmiCategory(record.BMICategory),
            SleepRecordId: sleepRecordId
        );
    }

    private static string MapBmiCategory(BMICategory bmi) => bmi switch
    {
        BMICategory.Underweight => "Underweight",
        BMICategory.Healthy_Weight => "Normal",
        BMICategory.Overweight => "Overweight",
        BMICategory.Obesity => "Obese",
        BMICategory.Obesity_Class_1 => "Obese",
        BMICategory.Obesity_Class_2 => "Obese",
        BMICategory.Obesity_Class_3 => "Obese",
        _ => "Normal"
    };
    private static string MapGender(string gender)
    {
        return gender switch
        {
            "F" => "Female",
            "M" => "Male",
            _ => "",
        };
    }
}