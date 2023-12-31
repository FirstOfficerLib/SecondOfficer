using Amazon.Lambda.Core;

namespace SecondOfficer.Lambda.Serialization
{
    internal class LambdaJsonSerializer : ILambdaSerializer
    {
        public LambdaJsonSerializer()
        {
        }

        public T Deserialize<T>(Stream requestStream)
        {
            throw new NotImplementedException();
        }

        public void Serialize<T>(T response, Stream responseStream)
        {
            throw new NotImplementedException();
        }
    }
}
