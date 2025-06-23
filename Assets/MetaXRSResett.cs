// Este script ayuda a asegurar que el Meta XR Simulator se reinicie limpiamente
// entre sesiones de Play en el Editor de Unity, incluyendo la reconexión de controladores físicos.
// Adjúntalo a un GameObject vacío en tu escena (puedes llamarlo "MetaXRSResetter").
// Asegúrate de que este GameObject exista solo en las escenas que usas para simular.

using UnityEngine;
#if UNITY_EDITOR // Solo para código que se ejecuta en el Editor de Unity
using UnityEditor;
using System;
using System.Reflection; // Necesario para reflexión
#endif

public class MetaXRSessionResetter : MonoBehaviour
{
#if UNITY_EDITOR
    private static Type _simulatorToolbarItemType;
    private static MethodInfo _activateSimulatorMethod;
    private static MethodInfo _deactivateSimulatorMethod;

    private static Type _physicalControllerConnectorType;
    private static MethodInfo _connectPhysicalControllersMethod;
    private static MethodInfo _disconnectPhysicalControllersMethod;

    /// <summary>
    /// Awake se llama cuando el script se carga.
    /// Se suscribe al evento playModeStateChanged para manejar el estado del simulador.
    /// </summary>
    private void Awake()
    {
        EditorApplication.playModeStateChanged += HandlePlayModeStateChange;
        Debug.Log("MetaXRSessionResetter: Suscrito al evento playModeStateChanged.");

        // Inicializamos los componentes de reflexión una vez
        InitializeReflection();

        // Aseguramos que el simulador y los controladores físicos estén activos/conectados
        // cuando entramos en Play Mode (en caso de que el script se adjunte o ya esté en la escena).
        EnsureSimulatorActiveAndControllersConnected();
    }

    /// <summary>
    /// OnDestroy se llama cuando el GameObject con este script es destruido.
    /// Nos desuscribimos del evento para evitar errores y referencias nulas.
    /// </summary>
    private void OnDestroy()
    {
        EditorApplication.playModeStateChanged -= HandlePlayModeStateChange;
        Debug.Log("MetaXRSessionResetter: Desuscrito del evento playModeStateChanged.");
    }

    /// <summary>
    /// Inicializa los elementos de reflexión para encontrar y llamar a los métodos del Meta XR Simulator.
    /// </summary>
    private void InitializeReflection()
    {
        if (_simulatorToolbarItemType != null && _physicalControllerConnectorType != null)
            return; // Ya inicializado

        // Reflección para la activación/desactivación general del simulador
        _simulatorToolbarItemType = Type.GetType("Meta.XR.Simulator.Editor.SimulatorToolbarItem, Meta.XR.Simulator.Editor");
        if (_simulatorToolbarItemType != null)
        {
            _activateSimulatorMethod = _simulatorToolbarItemType.GetMethod("Activate", BindingFlags.Public | BindingFlags.Static);
            _deactivateSimulatorMethod = _simulatorToolbarItemType.GetMethod("Deactivate", BindingFlags.Public | BindingFlags.Static);
        }
        else
        {
            Debug.LogWarning("MetaXRSessionResetter: No se encontró el tipo 'SimulatorToolbarItem'. El simulador puede no estar instalado o la versión ha cambiado.");
        }

        // Reflección para la conexión/desconexión de los controladores físicos
        _physicalControllerConnectorType = Type.GetType("Meta.XR.Simulator.Editor.PhysicalControllers.PhysicalControllerConnector, Meta.XR.Simulator.Editor");
        if (_physicalControllerConnectorType != null)
        {
            // Estos métodos pueden ser estáticos o requerir una instancia Singleton.
            // Para la mayoría de las herramientas de editor, suelen ser estáticos para comodidad.
            _connectPhysicalControllersMethod = _physicalControllerConnectorType.GetMethod("Connect", BindingFlags.Public | BindingFlags.Static);
            _disconnectPhysicalControllersMethod = _physicalControllerConnectorType.GetMethod("Disconnect", BindingFlags.Public | BindingFlags.Static);

            // Si no se encuentran como estáticos, podría ser más complejo (requerir encontrar una instancia singleton).
            // Para depuración, podríamos intentar un Init si existe (aunque no se va a invocar).
            // MethodInfo initMethod = _physicalControllerConnectorType.GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Static);
            // if (initMethod != null) initMethod.Invoke(null, null); // Esto podría ser necesario si no se inicializa automáticamente

            if (_connectPhysicalControllersMethod == null || _disconnectPhysicalControllersMethod == null)
            {
                 Debug.LogWarning("MetaXRSessionResetter: No se encontraron los métodos 'Connect' o 'Disconnect' para PhysicalControllerConnector como estáticos. La reconexión automática de controladores físicos podría no funcionar.");
            }
        }
        else
        {
            Debug.LogWarning("MetaXRSessionResetter: No se encontró el tipo 'PhysicalControllerConnector'. La funcionalidad de conexión de controladores físicos puede haber cambiado o no está disponible.");
        }
    }

    /// <summary>
    /// Maneja los cambios en el estado del modo Play.
    /// </summary>
    /// <param name="state">El nuevo PlayModeStateChange.</param>
    private void HandlePlayModeStateChange(PlayModeStateChange state)
    {
        // Debug.Log($"MetaXRSessionResetter: Play mode state changed to: {state}");

        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Justo antes de entrar en Play Mode, asegura que el simulador y los controladores estén activos.
            Debug.Log("MetaXRSessionResetter: Entrando en Play Mode. Asegurando que Meta XR Simulator y controladores estén activos.");
            EnsureSimulatorActiveAndControllersConnected();
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // Justo antes de salir de Play Mode, desactiva el simulador y desconecta los controladores.
            Debug.Log("MetaXRSessionResetter: Saliendo de Play Mode. Desactivando Meta XR Simulator y desconectando controladores.");
            DeactivateSimulatorAndControllers();
        }
    }

    /// <summary>
    /// Activa el Meta XR Simulator y, si es posible, conecta los controladores físicos.
    /// </summary>
    private void EnsureSimulatorActiveAndControllersConnected()
    {
        // 1. Activar la sesión general del Simulador
        if (_activateSimulatorMethod != null)
        {
            try
            {
                _activateSimulatorMethod.Invoke(null, null);
                Debug.Log("Meta XR Simulator: Activado via script.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Meta XR Simulator: Fallo al invocar método Activate. Error: {e.Message}");
            }
        }

        // 2. Intentar conectar controladores físicos
        if (_connectPhysicalControllersMethod != null)
        {
            try
            {
                _connectPhysicalControllersMethod.Invoke(null, null); // Invoke estático
                Debug.Log("Meta XR Simulator: Intentando conectar controladores físicos via script.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Meta XR Simulator: Fallo al invocar método ConnectPhysicalControllers. Error: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Meta XR Simulator: Método ConnectPhysicalControllers no encontrado. No se pueden conectar los controladores automáticamente.");
        }
    }

    /// <summary>
    /// Desactiva el Meta XR Simulator y, si es posible, desconecta los controladores físicos.
    /// </summary>
    private void DeactivateSimulatorAndControllers()
    {
        // 1. Desconectar controladores físicos
        if (_disconnectPhysicalControllersMethod != null)
        {
            try
            {
                _disconnectPhysicalControllersMethod.Invoke(null, null); // Invoke estático
                Debug.Log("Meta XR Simulator: Intentando desconectar controladores físicos via script.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Meta XR Simulator: Fallo al invocar método DisconnectPhysicalControllers. Error: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Meta XR Simulator: Método DisconnectPhysicalControllers no encontrado. No se pueden desconectar los controladores automáticamente.");
        }

        // 2. Desactivar la sesión general del Simulador
        if (_deactivateSimulatorMethod != null)
        {
            try
            {
                _deactivateSimulatorMethod.Invoke(null, null);
                Debug.Log("Meta XR Simulator: Desactivado via script.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Meta XR Simulator: Fallo al invocar método Deactivate. Error: {e.Message}");
            }
        }
    }
#endif // UNITY_EDITOR
}