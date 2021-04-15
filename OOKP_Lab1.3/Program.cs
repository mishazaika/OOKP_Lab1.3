using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OOKP_Lab1._3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ProducerConsumer pc = new ProducerConsumer();
            pc.StartChannel();
            Console.ReadKey();
        }
    }
    public class ProducerConsumer
    {
        string path = "C:/Users/misha/Desktop/Text.txt";
        static int messageLimit = 5;
        Channel<string> channel = Channel.CreateBounded<string>(messageLimit);
        public void StartChannel()
        {
            List<string> names = new List<string>();
            names.Add("Вага: 360 гр");
            names.Add("Ціна: 1000 грн");
            names.Add("Замовник: Піскун Александр Вікторович");
            names.Add("Дата: 20.05.2021");
            Task producer = Task.Factory.StartNew(() => {
                foreach (var name in names)
                {
                    channel.Writer.TryWrite(name);
                }
                channel.Writer.Complete();
            });
            Task[] consumer = new Task[1];
            for (int i = 0; i < consumer.Length; i++)
            {
                consumer[i] = Task.Factory.StartNew(async () => {
                    while (await channel.Reader.WaitToReadAsync())
                    {
                        if (channel.Reader.TryRead(out var data))
                        {
                            Console.WriteLine($" Data read from Consumer No.{Task.CurrentId} is {data}");
                            try
                            {
                                using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                                {
                                    await sw.WriteLineAsync(data);
                                }
                                Console.WriteLine("Запис виконано!");
   
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                });
            }

            producer.Wait();
            Task.WaitAll(consumer);
            
        }
    }
}
