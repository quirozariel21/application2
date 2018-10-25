using System;
using Confluent.Kafka;
using Models;
using Newtonsoft.Json;

class Program
{
    public static void Main(string[] args)
    {
        var conf = new ConsumerConfig{
            GroupId = "test-consumer-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetResetType.Earliest
        };

        using(var c =  new Consumer<Ignore, string>(conf)){
            c.Subscribe("test-topic");

            bool consuming = true;

            c.OnError += (_, e) => consuming = !e.IsFatal;

            while(consuming){
                try{
                    var cr = c.Consume();
                    Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                    string json = cr.Value.ToString();
                    Persona persona = JsonConvert.DeserializeObject<Persona>(json);
                    Console.WriteLine($"Nombre: {persona.nombre}");
                    Console.WriteLine($"Telefono: {persona.telefono}");

                }catch(ConsumeException e){
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
                }
            } 

            //Ensure the consumer leaves the group cleanly and final offsets are commited
            c.Close();   
        }
    }
}
