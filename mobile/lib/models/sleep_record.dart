class SleepRecord {
  const SleepRecord({
    this.id,
    required this.date,
    required this.sleepDuration,
    required this.sleepQuality,
    required this.stressLevel,
    required this.mentalFatigue,
    required this.physicalActivity,
    required this.steps,
    required this.heartRate,
    required this.bloodPressure,
    required this.screenTime,
    required this.caffeine,
    required this.alcohol,
    this.sleepScore,
    this.disorder,
  });

  final int? id;
  final String date;
  final int sleepDuration;
  final int sleepQuality;
  final int stressLevel;
  final int mentalFatigue;
  final String physicalActivity;
  final int steps;
  final int heartRate;
  final int bloodPressure;
  final int screenTime;
  final bool caffeine;
  final bool alcohol;
  final int? sleepScore;
  final String? disorder;

  factory SleepRecord.fromJson(Map<String, dynamic> json) {
    final durationInHours = _toDouble(json['durationInHours']);
    final durationInMinutes = durationInHours > 0
        ? (durationInHours * 60).round()
        : _toInt(json['sleepDuration']);

    return SleepRecord(
      id: _toIntOrNull(json['id'] ?? json['sleepRecordId']),
      date: (json['date'] ?? json['recordDate'] ?? '').toString(),
      sleepDuration: durationInMinutes,
      sleepQuality: _toInt(json['sleepQuality'] ?? json['qualityOfSleep']),
      stressLevel: _toInt(json['stressLevel']),
      mentalFatigue: _toInt(json['mentalFatigue']),
      physicalActivity: (json['physicalActivity'] ?? '').toString(),
      steps: _toInt(json['steps'] ?? json['dailySteps']),
      heartRate: _toInt(json['heartRate']),
      bloodPressure: _toInt(json['bloodPressure']),
      screenTime: json['screenBeforeSleep'] == true ? 1 : _toInt(json['screenTime']),
      caffeine: json['caffeine'] == true,
      alcohol: json['alcohol'] == true,
      sleepScore: _toIntOrNull(json['sleepScore']),
      disorder: json['disorder']?.toString(),
    );
  }

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{
      'date': date,
      'sleepDuration': sleepDuration,
      'sleepQuality': sleepQuality,
      'stressLevel': stressLevel,
      'mentalFatigue': mentalFatigue,
      'physicalActivity': physicalActivity,
      'steps': steps,
      'heartRate': heartRate,
      'bloodPressure': bloodPressure,
      'screenTime': screenTime,
      'caffeine': caffeine,
      'alcohol': alcohol,
    };

    if (id != null) json['id'] = id;
    if (sleepScore != null) json['sleepScore'] = sleepScore;
    if (disorder != null) json['disorder'] = disorder;
    return json;
  }

  Map<String, dynamic> toApiJson() {
    final start = DateTime.tryParse(date) ?? DateTime.now();
    final sleepStart = DateTime(start.year, start.month, start.day, 22);
    final sleepEnd = sleepStart.add(Duration(minutes: sleepDuration));

    return {
      'sleepStart': sleepStart.toIso8601String(),
      'sleepEnd': sleepEnd.toIso8601String(),
      'qualityOfSleep': sleepQuality,
      'stressLevel': stressLevel,
      'physicalActivityMinutes': _physicalActivityMinutes(physicalActivity),
      'bmiCategory': 1,
      'bloodPressure': bloodPressure,
      'notes': null,
      'heartRate': heartRate,
      'dailySteps': steps,
      'screenBeforeSleep': screenTime > 0,
      'caffeine': caffeine,
      'alcohol': alcohol,
      'mentalFatigue': mentalFatigue,
    };
  }

  static int _toInt(dynamic value) => _toIntOrNull(value) ?? 0;

  static int? _toIntOrNull(dynamic value) {
    if (value == null) return null;
    if (value is int) return value;
    if (value is num) return value.round();
    return int.tryParse(value.toString());
  }

  static double _toDouble(dynamic value) {
    if (value is num) return value.toDouble();
    return double.tryParse(value?.toString() ?? '') ?? 0;
  }

  static int _physicalActivityMinutes(String value) {
    switch (value) {
      case 'high':
        return 60;
      case 'medium':
        return 30;
      case 'low':
      default:
        return 0;
    }
  }
}
