using System.Collections.Generic;

namespace CalculatorWPF.Models
{
    public class MemoryModel
    {
        private readonly Stack<double> memoryStack = new();

        public void Clear() => memoryStack.Clear();

        public void Save(double value) => memoryStack.Push(value);

        public double? Recall() => memoryStack.Count > 0 ? memoryStack.Peek() : null;

        public void Add(double value)
        {
            if (memoryStack.Count > 0)
            {
                double last = memoryStack.Pop();
                memoryStack.Push(last + value);
            }
            else
            {
                memoryStack.Push(value);
            }
        }

        public void Subtract(double value)
        {
            if (memoryStack.Count > 0)
            {
                double last = memoryStack.Pop();
                memoryStack.Push(last - value);
            }
            else
            {
                memoryStack.Push(-value);
            }
        }

        public List<double> GetAllValues() => new(memoryStack);
    }
}
