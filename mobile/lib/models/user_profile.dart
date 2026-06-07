class UserProfile {
  const UserProfile({
    required this.name,
    required this.birthDate,
    required this.weight,
    required this.height,
    required this.gender,
    required this.sleepGoal,
    this.occupation = '',
  });

  final String name;
  final String birthDate;
  final double weight;
  final double height;
  final String gender;
  final double sleepGoal;
  final String occupation;

  factory UserProfile.fromJson(Map<String, dynamic> json) {
    return UserProfile(
      name: (json['name'] ?? '').toString(),
      birthDate: (json['birthDate'] ?? '').toString(),
      weight: _toDouble(json['weight']),
      height: _toDouble(json['height']),
      gender: (json['gender'] ?? '').toString(),
      sleepGoal: _toDouble(json['sleepGoal']),
      occupation: (json['occupation'] ?? json['profession'] ?? '').toString(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'birthDate': birthDate,
      'weight': weight,
      'height': height,
      'gender': gender,
      'sleepGoal': sleepGoal,
      'occupation': occupation,
    };
  }

  static double _toDouble(dynamic value) {
    if (value is num) return value.toDouble();
    return double.tryParse(value?.toString() ?? '') ?? 0;
  }
}
