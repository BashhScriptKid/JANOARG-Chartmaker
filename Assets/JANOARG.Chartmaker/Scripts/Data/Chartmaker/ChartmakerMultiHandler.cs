using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JANOARG.Chartmaker.Behaviors.Chartmaker;
using JANOARG.Chartmaker.Data.Chartmaker.MultiEdit;
using JANOARG.Chartmaker.Data.Chartmaker.Actions;
using JANOARG.Shared.Data.ChartInfo;
using UnityEngine;

namespace JANOARG.Chartmaker.Data.Chartmaker
{
    public class ChartmakerMultiManager
    {
        public Type target;

        public List<FieldInfo> AvailableFields;

        public int CurrentFieldIndex;

        public ChartmakerMultiHandler Handler;

        public Dictionary<Type, ChartmakerMultiHandler> Handlers = new Dictionary<Type, ChartmakerMultiHandler>();

        public ChartmakerMultiManager(Type type)
        {
            AvailableFields = new List<FieldInfo>();

            foreach (FieldInfo field in type.GetFields()) 
            {
                if (typeof(IEnumerable).IsAssignableFrom(field.FieldType)
                    || typeof(Storyboard) == field.FieldType
                    || field.IsStatic || field.IsLiteral || !field.IsPublic) 
                {
                    continue;
                }
                AvailableFields.Add(field);
            }

            target = type;
            SetTarget(0);
        }

        public void SetTarget(int target)
        {
            CurrentFieldIndex = target;

            FieldInfo currentField = AvailableFields[target];

            IList current = InspectorPanel.main.CurrentTimestamp?.Count > 1 
                ? InspectorPanel.main.CurrentTimestamp 
                : InspectorPanel.main.CurrentObject as IList;

            if (currentField.FieldType != Handler?.TargetType)
            {
                if (currentField.FieldType == typeof(bool)) 
                {
                    Handler = Handlers.ContainsKey(currentField.FieldType) 
                        ? Handlers[currentField.FieldType] 
                        : new ChartmakerMultiHandlerBoolean();
                }
                else if (currentField.FieldType == typeof(BeatPosition)) 
                {
                    ChartmakerMultiHandlerBeatPosition handler = Handlers.ContainsKey(currentField.FieldType)
                        ? Handlers[currentField.FieldType] as ChartmakerMultiHandlerBeatPosition
                        : new ChartmakerMultiHandlerBeatPosition();
               
                    handler.SetLerp(current);
                    Handler = handler;
                }
                else if (currentField.FieldType == typeof(float)) 
                {
                    ChartmakerMultiHandlerFloat handler = Handlers.ContainsKey(currentField.FieldType)
                        ? Handlers[currentField.FieldType] as ChartmakerMultiHandlerFloat 
                        : new ChartmakerMultiHandlerFloat();
                
                    handler.SetLerp(current);
                    Handler = handler;
                }
                else if (currentField.FieldType == typeof(Vector2)) 
                {
                    ChartmakerMultiHandlerVector2 handler = Handlers.ContainsKey(currentField.FieldType)
                        ? Handlers[currentField.FieldType] as ChartmakerMultiHandlerVector2 
                        : new ChartmakerMultiHandlerVector2();
                
                    handler.SetLerp(current);
                    Handler = handler;
                }
                else if (currentField.FieldType == typeof(Vector3)) 
                {
                    ChartmakerMultiHandlerVector3 handler = Handlers.ContainsKey(currentField.FieldType)
                        ? Handlers[currentField.FieldType] as ChartmakerMultiHandlerVector3 
                        : new ChartmakerMultiHandlerVector3();
                
                    handler.SetLerp(current);
                    Handler = handler;
                }
                else if (currentField.FieldType == typeof(Color)) 
                {
                    ChartmakerMultiHandlerColor handler = Handlers.ContainsKey(currentField.FieldType)
                        ? Handlers[currentField.FieldType] as ChartmakerMultiHandlerColor 
                        : new ChartmakerMultiHandlerColor();
                
                    handler.SetLerp(current);
                    Handler = handler;
                }
                else 
                {
                    Handler = Handlers.ContainsKey(currentField.FieldType) 
                        ? Handlers[currentField.FieldType] 
                        : Activator.CreateInstance(typeof(ChartmakerMultiHandler<>).MakeGenericType(currentField.FieldType)) as ChartmakerMultiHandler;
                }
            }
            Handlers[currentField.FieldType] = Handler;
        }

        public void Execute(IList items, ChartmakerHistory history) 
        {
            FieldInfo currentField = AvailableFields[CurrentFieldIndex];

            ChartmakerMultiEditAction action = new ChartmakerMultiEditAction() 
            { 
                Keyword = currentField.Name 
            };

            foreach(object obj in items) 
            {
                ChartmakerMultiEditActionItem item = new ChartmakerMultiEditActionItem
                {
                    Target = obj,
                    From = currentField.GetValue(obj),
                };
                item.To = Handler.Get(item.From, obj);
                action.Targets.Add(item);
            }
        
            action.Redo();
            history.ActionsBehind.Push(action);
            history.ActionsAhead.Clear();
        }
    }

    public abstract class ChartmakerMultiHandler
    {
        public object To;
    
        public virtual object Get(object from, object source, int index = 0) => To;

        public abstract Type TargetType { get; }
    }

    public class ChartmakerMultiHandler<T>: ChartmakerMultiHandler
    {
    
        public override object Get(object from, object source, int index = 0) 
            => Get((T)from, source, index);

        public virtual T Get(T from, object source, int index = 0) => (T)To;

        public override Type TargetType { get { return typeof(T); } }
    }
}