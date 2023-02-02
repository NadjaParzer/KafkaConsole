using ksqlDB.RestApi.Client.KSql.Linq;
using ksqlDB.RestApi.Client.KSql.Query;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.Query.Options;
using ksqlDB.RestApi.Client.KSql.RestApi.Http;
using ksqlDB.RestApi.Client.KSql.RestApi;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;
using ksqlDB.RestApi.Client.KSql.Query.Functions;
using System.Reactive.Concurrency;
using ksqlDB.RestApi.Client.KSql.Query.Context.Options;
using ksqlDB.RestApi.Client.KSql.RestApi.Parameters;
using ConsoleKSqlDB;
using System.Reactive.Linq;


#region
//var contextOptions = new KSqlDBContextOptions(ksqlDbUrl)
//{
//    ShouldPluralizeFromItemName = true
//};

//EntityCreationMetadata metadata = new()
//{
//    KafkaTopic = "tweets",
//    Partitions = 3,
//    Replicas = 1
//};

//var httpClient = new HttpClient()
//{
//    BaseAddress = new Uri(@"http:\\localhost:8088")
//};

//await using var context = new KSqlDBContext(contextOptions);



//var httpClientFactory = new HttpClientFactory(httpClient);
//var restApiClient = new KSqlDbRestApiClient(httpClientFactory);

//var httpResponseMessage = await restApiClient.CreateOrReplaceStreamAsync<Tweet>(metadata);



//var responseMessage = await new KSqlDbRestApiClient(httpClientFactory)
//  .InsertIntoAsync(new Tweet { Id = 2, Message = "ksqlDB rulez!" });

////context.Add(new Tweet { Id = 1, Message = "Hello world" });
////context.Add(new Tweet { Id = 3, Message = "ksqlDB rulez!" });
////var saveChangesResponse = await context.SaveChangesAsync();

//using var disposable = context.CreateQueryStream<Tweet>()
//  .WithOffsetResetPolicy(AutoOffsetReset.Latest)
//  .Where(p => p.Message != "Hello world" || p.Id == 1)
//  .Select(l => new { l.Message, l.Id })
//  .Take(2)
//  .Subscribe(tweetMessage =>
//  {
//      Console.WriteLine($"testKsql: {tweetMessage.Id} - {tweetMessage.Message}");
//  }, error => { Console.WriteLine($"Exception: {error.Message}"); }, () => Console.WriteLine("Completed"));

//Console.WriteLine("Press any key to stop the subscription");

//Console.ReadKey();

//public class Tweet : Record
//{
//    public int Id { get; set; }
//    public string Message { get; set; }
//}
#endregion

//namespace kSqlDB.RestApi
//{
//    class Program
//    {

//    }
//}

var ksqlDbUrl = @"http:\\localhost:8088";
var contextOptions = new KSqlDbContextOptionsBuilder()
        .UseKSqlDb(ksqlDbUrl)
        .SetupQuery(options =>
        {
            options.Properties[QueryParameters.AutoOffsetResetPropertyName] = AutoOffsetReset.Earliest.ToString().ToLower(); // "latest"
        })
        .Options;


var context = new KSqlDBContext(contextOptions);



var subscription = context.CreateQuery<Movie>()
        .Where(p => p.Title != "E.T.")
        //.Where(c => K.Functions.Like(c.Title.ToLower(), "%hard%".ToLower()) || c.Id == 1)
        //.Where(p => p.RowTime >= 1510923225000) //AND RowTime >= 1510923225000
        .Select(l => new { Id2 = l.Id, l.Title, l.Release_Year, l.RowTime })
        //.Take(2) // LIMIT 2    
        .ToObservable() // client side processing starts here lazily after subscription. Switches to Rx.NET
        .Finally(() => { Console.WriteLine("Finally"); })
        .ObserveOn(TaskPoolScheduler.Default)
        .Subscribe(onNext: movie =>
        {
            Console.WriteLine($"{nameof(Movie)}: {movie.Id2} - {movie.Title} - {movie.RowTime}");
            context.CreateQueryStream<Movie>("newStream")
            .WithOffsetResetPolicy(AutoOffsetReset.Latest)
            .Select(m => new { m.Id, m.Title, });
            Console.WriteLine();
        }, onError: error => { Console.WriteLine($"Exception: {error.Message}"); }, onCompleted: () => Console.WriteLine("Completed"));


Console.ReadLine();
