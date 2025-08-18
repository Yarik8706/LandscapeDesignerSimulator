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
        public string[] clientsNames;
        public Sprite[] clientsAvatars;

        [TextArea] public string firstSystemMessage = "Поприветствуй игрока и изложи, что ты хочешь заказать для своей территории (по профилю клиента и варианту требований).";
        [TextArea] public string playerProgressQuestion = "Выведи игроку, сколько пунктов он уже узнал из требований и сколько всего нужно выяснить. Формат: «X / Y».";
        [TextArea] public string gottenPlayerTasks = "Перечисли пункты, которые игрок уже выяснил на данный момент (бюджет, срок, обязательные элементы, дополнительные пожелания, приоритет).";
    }
}