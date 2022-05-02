namespace Ninject.Tests.Unit
{
    public class TestObject
    {
        private int value;

        public TestObject(int value)
        {
            this.value = value;
        }

        public override int GetHashCode()
        {
            return this.value;
        }

        public override bool Equals(object obj)
        {
            var other = obj as TestObject;
            if (other != null)
            {
                return other.value.Equals(this.value);
            }

            return this.value.Equals(obj);
        }

        public void ChangeHashCode(int i)
        {
            this.value = i;
        }
    }
}