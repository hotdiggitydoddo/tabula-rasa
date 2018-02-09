using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using TabulaRasa.Data;
using TabulaRasa.Services;
using TabulaRasa.Test.Admin;

namespace TabulaRasa.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", true, true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkNpgsql()
                .AddDbContext<TRDbContext>(options => options.UseNpgsql(config.GetConnectionString("default")))
                .AddSingleton<IGame, Game>()
                .AddTransient<IEntityService, EntityService>()
                .AddTransient<IScriptService, ScriptService>()
                .AddTransient<IComponentService, ComponentService>()
                .AddTransient<ITemplateService, TemplateService>()
                .AddTransient<ITimedActionService, TimedActionService>()
                .AddTransient<ILocaleService, LocaleService>()
                .BuildServiceProvider();

            var game = serviceProvider.GetService<IGame>();
            game.Start();

            var admin = new AdminConsole(game);

            while (game.IsRunning)
            {
                admin.HandleInput(Console.ReadLine());
            }
        }


        //static void Main(string[] args)
        //{
        //    IScriptManager scripts = new ScriptManager();
        //    IComponentManager components = new ComponentManager(scripts);
        //    IEntityManager entities = new EntityManager();
        //    IActionManager actions = new ActionManager();
        //    ILocationManager locations = new LocationManager();



        //    GameWrapper g = new GameWrapper(entities, components, actions, locations);

        //    scripts.RegisterObjectType<Entity>();
        //    scripts.RegisterObjectType<Component>();
        //    scripts.RegisterObjectType<GameWrapper>();
        //    scripts.RegisterObjectType<TraitSet>();
        //    scripts.RegisterObjectType<Script>();
        //    scripts.RegisterObjectType<GameAction>();
        //    scripts.RegisterObjectType<TimedGameAction>();


        //    //create entities -- these should be done through game actions, not function calls on the gamewrapper.
        //    //EVERYTHING should be through actions and the gamewrapper processing them
        //    var snake = g.CreateEntity("snake");
        //    g.HandleAction(new GameAction("addcomponent", snake.Id, "health", @"{""maxHP"":150, ""currHP"":22}"));

        //    var angel = g.CreateEntity("angel");
        //    g.HandleAction(new GameAction("addcomponent", angel.Id, "health", @"{""maxHP"":300, ""currHP"":300}"));
        //    g.HandleAction(new GameAction("addcomponent", angel.Id, "angel-ai"));

        //    //stick em in a room -- through game actions, of course!!!

            




        //    //g.HandleAction(new GameAction("addcomponent", e.Id, "invulnerability"));
        //    actions.Add(new TimedGameAction(DateTime.Now.AddSeconds(5).ToString(), "do-takedamage", 0, snake.Id, "300", "entity"));
        //    actions.Add(new TimedGameAction(DateTime.Now.AddSeconds(7).ToString(), "do-heal", angel.Id, snake.Id, "500", "entity"));
        //    // g.HandleAction(new GameAction("messageentity", 0, e.Id, "health", "takedamage", "300"));
        //    // g.HandleAction(new GameAction("messagecomponent", 0, e.Id, "health", "takedamage", "300"));
        //    // g.HandleAction(new GameAction("messagecomponent", 0, e.Id, "health", "heal", "325"));
        //    //g.HandleAction(new GameAction("heal", 0, e.Id, "325"));
        //    //g.HandleAction(new GameAction("removecomponent", e.Id, "health"));

        //    while (g.IsRunning)
        //    {

        //    }
        //}
    }
}
