using System;
using UnityEngine;

namespace Infra
{
    [Serializable]
    [CreateAssetMenu(fileName = "ClientsData", menuName = "Infra/ClientsData")]
    public class ClientsData : ScriptableObject
    {
        [TextArea] public string generalContext;
        [TextArea] public string[] personas;
        [TextArea] public string[] clientsBriefs;
        [TextArea] public string secondAITask;
        [TextArea] public string finalAITask;
        [TextArea] public string setResultAITask;
        public string[] clientsNames;
        public Sprite[] clientsAvatars;

        [TextArea] public string firstSystemMessage = "Поприветствуй игрока и изложи, что ты хочешь заказать для своей территории (по профилю клиента и варианту требований).";
    }
}