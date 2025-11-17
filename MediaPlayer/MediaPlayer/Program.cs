/* .NET Core 9 Web App  */
using System.Text.Json;
using MediaPlayer;

(JsonDocument? node, Exception? error)? document = JDocument.ParseDocument(AppContext.BaseDirectory, "environment.json");

var response = JDocument.Get<string?>("environment", document?.node);

var environment = response.Item1 ?? Environments.Development;

AppGenerator.Start(environment, args);