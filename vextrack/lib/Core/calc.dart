class Calc
{
  static int cumulativeSum(int iterations, int offset, int step)
  {
    int sum = 0;
    for(int i = 2; i <= iterations; i++)
    {
      sum += (i * step) + offset;
    }
    return sum;
  }
}