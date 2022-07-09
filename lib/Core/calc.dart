class Calc
{
  static int cumulativeSum(int iterations, int offset, int step)
  {
    int sum = offset;
    for(int i = 0; i < iterations; i++)
    {
      sum += i * step;
    }
    return sum;
  }
}