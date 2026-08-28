namespace PowerToolbox.Extensions.Hashing
{
    internal struct InternalState
    {
        internal uint A, B, C, D;
        internal long hashedLength;
        internal byte[] Buffer;

        internal InternalState(long hashedLength, uint A, uint B, uint C, uint D, byte[] Buffer)
        {
            this.hashedLength = hashedLength;
            this.A = A;
            this.B = B;
            this.C = C;
            this.D = D;
            this.Buffer = (byte[])Buffer.Clone();
        }
    }
}
