class Insights {
  const Insights({
    required this.averageSleep,
    required this.averageScore,
    required this.patterns,
    required this.recommendations,
  });

  final double averageSleep;
  final double averageScore;
  final List<String> patterns;
  final List<String> recommendations;

  factory Insights.fromJson(Map<String, dynamic> json) {
    return Insights(
      averageSleep: _toDouble(json['averageSleep']),
      averageScore: _toDouble(json['averageScore']),
      patterns: _toStringList(json['patterns']),
      recommendations: _toStringList(json['recommendations']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'averageSleep': averageSleep,
      'averageScore': averageScore,
      'patterns': patterns,
      'recommendations': recommendations,
    };
  }

  static double _toDouble(dynamic value) {
    if (value is num) return value.toDouble();
    return double.tryParse(value?.toString() ?? '') ?? 0;
  }

  static List<String> _toStringList(dynamic value) {
    if (value is List) {
      return value.map((item) => item.toString()).toList();
    }
    return const [];
  }
}
