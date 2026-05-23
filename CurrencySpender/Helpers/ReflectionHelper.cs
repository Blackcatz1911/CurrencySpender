using System;
using System.Reflection;
using Dalamud.Plugin;
using Serilog;

namespace CurrencySpender.Helpers;

public static class ReflectionHelper
{
    private const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    public static object? GetFoP(this object obj, string name)
    {
        var type = obj.GetType();
        while (type != null)
        {
            var fieldInfo = type.GetField(name, AllFlags);
            if (fieldInfo != null)
                return fieldInfo.GetValue(obj);

            var propertyInfo = type.GetProperty(name, AllFlags);
            if (propertyInfo != null)
                return propertyInfo.GetValue(obj);

            type = type.BaseType;
        }
        return null;
    }

    public static T? GetFoP<T>(this object obj, string name)
        => (T?)GetFoP(obj, name);

    public static bool TryGetDalamudPlugin(string internalName, out IDalamudPlugin? instance)
    {
        try
        {
            var pluginManager = GetPluginManager();
            if (pluginManager == null)
            {
                instance = null;
                return false;
            }

            var installedPlugins = (System.Collections.IList?)pluginManager.GetType().GetProperty("InstalledPlugins")?.GetValue(pluginManager);
            if (installedPlugins == null)
            {
                instance = null;
                return false;
            }

            foreach (var plugin in installedPlugins)
            {
                var pluginInternalName = (string?)plugin.GetType().GetProperty("InternalName")?.GetValue(plugin);
                if (pluginInternalName == internalName)
                {
                    var type = plugin.GetType().Name == "LocalDevPlugin" ? plugin.GetType().BaseType : plugin.GetType();
                    if (type == null)
                    {
                        instance = null;
                        return false;
                    }

                    var pluginInstance = (IDalamudPlugin?)type.GetField("instance", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(plugin);
                    if (pluginInstance != null)
                    {
                        instance = pluginInstance;
                        return true;
                    }
                }
            }

            instance = null;
            return false;
        }
        catch (Exception e)
        {
            Log.Debug($"Failed to get plugin {internalName}: {e.Message}");
            instance = null;
            return false;
        }
    }

    private static object? GetPluginManager()
    {
        try
        {
            var dalamudAssembly = Service.PluginInterface.GetType().Assembly;
            var serviceType = dalamudAssembly.GetType("Dalamud.Service`1", true);
            var pluginManagerType = dalamudAssembly.GetType("Dalamud.Plugin.Internal.PluginManager", true);
            
            if (serviceType == null || pluginManagerType == null)
                return null;

            var genericServiceType = serviceType.MakeGenericType(pluginManagerType);
            var getMethod = genericServiceType.GetMethod("Get");
            
            return getMethod?.Invoke(null, BindingFlags.Default, null, Array.Empty<object>(), null);
        }
        catch
        {
            return null;
        }
    }
}
