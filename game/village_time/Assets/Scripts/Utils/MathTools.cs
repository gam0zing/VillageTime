public static class MathTools {
    public static double DoubleClamp(double value, double min, double max) { 
        if (value < min) { value = min; }
        else if (value > max) { value = max; }
        return value;
    }
}