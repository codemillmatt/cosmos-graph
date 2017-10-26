using System;

using UIKit;

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Graphs;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace Graph
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            gremlinButton.TouchUpInside += async (sender, e) =>
            {
                try
                {
                    string endpoint = "";
                    string primaryKey = "";

                    string dbName = "graphdb";
                    string collectionName = "Persons";

                    using (var client = new DocumentClient(new Uri(endpoint), primaryKey))
                    {
                        var database = new Database { Id = dbName };
                        await client.CreateDatabaseIfNotExistsAsync(database);

                        var dbUri = UriFactory.CreateDatabaseUri(dbName);

                        var docCollection = new DocumentCollection { Id = collectionName };
                        await client.CreateDocumentCollectionIfNotExistsAsync(dbUri, docCollection);

                        IDocumentQuery<dynamic> gremlinQuery = client.CreateGremlinQuery<dynamic>(docCollection, "g.V().count()");

                        while (gremlinQuery.HasMoreResults)
                        {
                            var result = JsonConvert.SerializeObject(await gremlinQuery.ExecuteNextAsync());
                            Console.WriteLine(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
