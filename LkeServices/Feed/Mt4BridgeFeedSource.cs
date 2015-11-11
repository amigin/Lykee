using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Core;
using Core.Feed;
using TcpSockets;

namespace LkeServices.Feed
{

    public class Mt4BridgeQuoteSerializer : ISocketSerializer
    {



        private readonly Action<AssetQuote> _quoteCallback;
        private readonly List<byte> _buffer = new List<byte>();


        private static DateTime ParseDateTime(string src)
        {
            var year = int.Parse(src.Substring(0, 4));
            var month = int.Parse(src.Substring(4, 2));
            var day = int.Parse(src.Substring(6, 2));
            var hour = int.Parse(src.Substring(8, 2));
            var min = int.Parse(src.Substring(10, 2));
            var sec = int.Parse(src.Substring(12, 2));

            return new DateTime(year, month, day, hour, min, sec).AddHours(GlobalSettings.Mt4TimeOffset);
        }

        private static AssetQuote CreateFeedData(string src)
        {
            var lines = src.Split(' ');

            if (lines.Length < 4)
                return null;

            var result = new AssetQuote
            {
                Id = lines[0],
                DateTime = ParseDateTime(lines[1]),
                Bid = double.Parse(lines[2], CultureInfo.InvariantCulture),
                Ask = double.Parse(lines[3], CultureInfo.InvariantCulture)
            };

            return result;
        }

        public void Init()
        {
            _buffer.Clear();
        }

        public void Deserialize(IEnumerable<byte> data)
        {
            _buffer.AddRange(data.Where(i => i != 0));
            HandleData();
        }

        private static readonly byte[] PingPacket = Encoding.ASCII.GetBytes("ping"+(char)13+ (char)10); 

        public byte[] CreatePingPacket()
        {
            return PingPacket;
        }

        private static readonly byte[] Clcr = { 13, 10 };

        private void HandleData()
        {
            //var i = _buffer.IndexOf(Clcrz);

          //  if (i == -1)
              var  i = _buffer.IndexOf(Clcr);

            while (i != -1)
            {
                var str = Encoding.ASCII.GetString(_buffer.CutFrom(0, i).ToArray());

                var feedData = CreateFeedData(str);

                if (feedData != null)
                    _quoteCallback(feedData);

                _buffer.RemoveRange(0, i+2);
                i = _buffer.IndexOf(Clcr);

            }

        }



        public Mt4BridgeQuoteSerializer(Action<AssetQuote> quoteCallback)
        {
            _quoteCallback = quoteCallback;
        }
    }



    public class Mt4BridgeFeedSource : TimerPeriod
    {

        public SocketLogInMemory SocketLog { get; }

        private readonly Dictionary<string, string> _ourQuotes = new Dictionary<string, string>
        {
            {".AUS200","AUS200"},
            {".DE30","DE30"},
            {".F40","F40"},
            {".JP225","JP225"},
            {".UK100","UK100"},
            {".US30","US30"},
            {".US500","US500"},

        };

        private Action<AssetQuote[]> _feedDatas;

        private readonly Dictionary<string, AssetQuote> _feed = new Dictionary<string, AssetQuote>(); 

        private readonly SimpleClientTcpSocket _simpleTcpSocket;


        private readonly SortedDictionary<string, AssetQuote> _profile = new SortedDictionary<string, AssetQuote>(); 


        public void RegisterFeed(Action<AssetQuote[]> feedDatas)
        {
            _feedDatas = feedDatas;
        }

        public Mt4BridgeFeedSource(IPEndPoint ipEndPoint, ILog log) : base("Mt4BridgeFeedSource", 300, log)
        {
            var mt4BridgeQuoteSerializer = new Mt4BridgeQuoteSerializer(QuoteCallBack);
            SocketLog = new SocketLogInMemory();
            _simpleTcpSocket = new SimpleClientTcpSocket("Mt4ManagerFeed", ipEndPoint, 3000, mt4BridgeQuoteSerializer, SocketLog, 10, 3);
        }

        public override void Start()
        {
            base.Start();
            _simpleTcpSocket.Start();
        }

        private void QuoteCallBack(AssetQuote feed)
        {
            lock (_feed)
            {
                if (!_feed.ContainsKey(feed.Id))
                    _feed.Add(feed.Id, feed);
                _feed[feed.Id] = feed;
            }

            lock (_profile)
            {

                if (_profile.ContainsKey(feed.Id))
                    _profile[feed.Id] = feed;
                else
                    _profile.Add(feed.Id, feed);
            }
        }

        public int ProfileCount()
        {
            lock (_profile)
                return _profile.Count;
        }

        public AssetQuote[] GetProfile()
        {
            lock (_profile)
                return _profile.Values.ToArray();
        }

        protected override Task Execute()
        {
            return Task.Run(() =>
            {
                var result = new List<AssetQuote>();

                lock (_feed)
                {
                    result.AddRange(from feedData in _feed.Values
                        let assetName =
                            _ourQuotes.ContainsKey(feedData.Id) ? _ourQuotes[feedData.Id] : feedData.Id
                        select new AssetQuote
                        {
                            Id = assetName,
                            Ask = feedData.Ask,
                            Bid = feedData.Bid,
                            DateTime = feedData.DateTime,
                        });

                    _feed.Clear();
                }

                if (result.Count > 0)
                    _feedDatas?.Invoke(result.Where(itm =>  GlobalSettings.FinanceInstruments.ContainsKey(itm.Id)).ToArray());

            });
        }
    }
}
