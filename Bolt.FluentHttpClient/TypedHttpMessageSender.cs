using Bolt.FluentHttpClient.Abstracts;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public class TypedHttpMessageSender : ITypedHttpMessageSender
    {
        private readonly IHttpMessageSerializer _serializer;
        private readonly IHttpMessageSender _sender;

        public TypedHttpMessageSender(IHttpMessageSerializer serializer, IHttpMessageSender sender)
        {
            _serializer = serializer;
            _sender = sender;
        }

        public async Task<TypedHttpMessageOutput> SendAsync(TypedHttpMessageInput input)
        {
            var msgInput = HttpMessageInputFactory.Create(input);

            var rsp = await _sender.SendAsync(msgInput);

            return await TypedHttpMessageOutputFactory.Create(rsp);
        }

        public async Task<TypedHttpMessageOutput<TOutput>> SendAsync<TOutput>(TypedHttpMessageInput input)
        {
            var msgInput = HttpMessageInputFactory.Create(input);

            var rsp = await _sender.SendAsync(msgInput);

            return await TypedHttpMessageOutputFactory.Create<TOutput>(rsp, _serializer);
        }

        public async Task<TypedHttpMessageOutput> SendAsync<TInput>(TypedHttpMessageInput<TInput> input)
        {
            using (var ms = new MemoryStream())
            using (var msgInput = HttpMessageInputFactory.Create(input, ms, _serializer))
            {
                var rsp = await _sender.SendAsync(msgInput);

                return await TypedHttpMessageOutputFactory.Create(rsp);
            }
        }

        public async Task<TypedHttpMessageOutput<TOutput>> SendAsync<TInput, TOutput>(TypedHttpMessageInput<TInput> input)
        {
            using (var ms = new MemoryStream())
            using (var msgInput = HttpMessageInputFactory.Create(input, ms, _serializer))
            {
                var rsp = await _sender.SendAsync(msgInput);

                return await TypedHttpMessageOutputFactory.Create<TOutput>(rsp, _serializer);
            }
        }
    }
}
