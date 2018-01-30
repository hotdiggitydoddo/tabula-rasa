using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using TabulaRasa.Core.Contracts;
using TabulaRasa.Core.Helpers;

namespace TabulaRasa.Core.Managers
{
    public class ScriptManager : IScriptManager
    {
        private readonly Dictionary<string, string> _gameFlowScripts;
        private readonly Dictionary<string, string> _componentScripts;
        private readonly Dictionary<string, string> _actionRunnerScripts;
        private readonly Dictionary<string, string> _commandScripts;

        public ScriptManager()
        {
            _gameFlowScripts = new Dictionary<string, string>();
            _componentScripts = new Dictionary<string, string>();
            _actionRunnerScripts = new Dictionary<string, string>();
            _commandScripts = new Dictionary<string, string>();
            Init();
        }

        public void RegisterObjectType<T>()
        {
            UserData.RegisterType<T>();
        }

        public string GetComponentScript(string name)
        {
            return _componentScripts.ContainsKey(name) ? _componentScripts[name] : throw new KeyNotFoundException($"{name} is not a valid component.");
        }
        private void Init()
        {
            _gameFlowScripts.Clear();
            _commandScripts.Clear();
            _componentScripts.Clear();
            _actionRunnerScripts.Clear();

            RefreshScriptsofType(ScriptType.Command);
            RefreshScriptsofType(ScriptType.Component);
            RefreshScriptsofType(ScriptType.ActionRunner);
            RefreshScriptsofType(ScriptType.GameFlow);
        }

        private void RefreshScriptsofType(ScriptType t)
        {
            var files = new List<string>();

            files.AddRange(Directory.GetFiles(Path.Combine("Scripts", $"{Enum.GetName(typeof(ScriptType), t)}"), "*.lua", SearchOption.AllDirectories));
            foreach (var file in files)
            {
                var name = @file.Substring(@file.LastIndexOf(Path.DirectorySeparatorChar) + 1,
                    @file.LastIndexOf(".") - @file.LastIndexOf(Path.DirectorySeparatorChar) - 1);
                var script = System.IO.File.ReadAllText(file);

                switch (t)
                {
                    case ScriptType.Command:
                        _commandScripts.Add(name, script);
                        break;
                    case ScriptType.Component:
                        _componentScripts.Add(name, script);
                        break;
                    case ScriptType.ActionRunner:
                        _actionRunnerScripts.Add(name, script);
                        break;
                    case ScriptType.GameFlow:
                        _gameFlowScripts.Add(name, script);
                        break;
                }
            }
        }
    }
}