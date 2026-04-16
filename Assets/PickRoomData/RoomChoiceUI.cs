using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomChoiceUI : MonoBehaviour
{
    [Header("Top Text")]
    [SerializeField] private TMP_Text roomCountText;
    [SerializeField] private TMP_Text chooseRoomText;

    [Header("Room Cards")]
    [SerializeField] private Button[] roomButtons;
    [SerializeField] private TMP_Text[] roomTypeTexts;
    [SerializeField] private TMP_Text[] roomDetailTexts;

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (RunManager.Instance == null)
        {
            Debug.LogError("RoomChoiceUI: RunManager missing.");
            return;
        }

        if (RunManager.Instance.currentOfferSet == null)
        {
            Debug.LogError("RoomChoiceUI: currentOfferSet missing.");
            return;
        }

        if (roomCountText != null)
        {
            int roomNumber = Mathf.Min(RunManager.Instance.roomsCleared + 1, 11);
            roomCountText.text = roomNumber <= 10
                ? $"ROOM {roomNumber}"
                : "BOSS";
        }

        if (chooseRoomText != null)
            chooseRoomText.text = "Choose a Room";

        if (RunManager.Instance.roomsCleared == 0)
            chooseRoomText.text = "Begin Your Journey";

        // Hide all first
        for (int i = 0; i < roomButtons.Length; i++)
        {
            if (roomButtons[i] != null)
                roomButtons[i].gameObject.SetActive(false);
        }

        int offerCount = RunManager.Instance.currentOfferSet.options.Count;

        // First room / single-offer layout -> middle card only
        if (offerCount == 1)
        {
            SetupCard(1, RunManager.Instance.currentOfferSet.options[0]);
            return;
        }

        // Normal 3-offer layout
        for (int i = 0; i < offerCount && i < roomButtons.Length; i++)
        {
            SetupCard(i, RunManager.Instance.currentOfferSet.options[i]);
        }
    }

    private void SetupCard(int cardIndex, RoomData room)
    {
        if (cardIndex < 0 || cardIndex >= roomButtons.Length)
            return;

        roomButtons[cardIndex].gameObject.SetActive(true);

        if (roomTypeTexts != null && cardIndex < roomTypeTexts.Length && roomTypeTexts[cardIndex] != null)
            roomTypeTexts[cardIndex].text = GetRoomTypeLabel(room);

        if (roomDetailTexts != null && cardIndex < roomDetailTexts.Length && roomDetailTexts[cardIndex] != null)
            roomDetailTexts[cardIndex].text = BuildRoomDetails(room);

        roomButtons[cardIndex].onClick.RemoveAllListeners();
        roomButtons[cardIndex].onClick.AddListener(() =>
        {
            Debug.Log($"Selected room: {room.roomType}");
            RunManager.Instance.SelectRoom(room);
        });
    }

    private string GetRoomTypeLabel(RoomData room)
    {
        switch (room.roomType)
        {
            case RoomType.Combat: return "COMBAT";
            case RoomType.Heal: return "HEAL";
            case RoomType.Shop: return "SHOP";
            case RoomType.Recruit: return "RECRUIT";
            case RoomType.Boss: return "BOSS";
            default: return "UNKNOWN";
        }
    }

    private string BuildRoomDetails(RoomData room)
    {
        if (room == null)
            return "Unknown room.";

        string text = room.flavorText;

        if ((room.roomType == RoomType.Combat || room.roomType == RoomType.Boss) &&
            room.enemies != null &&
            room.enemies.Length > 0)
        {
            text += "\n\nEnemies:";
            for (int i = 0; i < room.enemies.Length; i++)
                text += $"\n- {room.enemies[i]}";
        }

        if (room.roomType == RoomType.Heal)
            text += "\n\nRestore health.";

        if (room.roomType == RoomType.Recruit)
            text += "\n\nA possible ally may be found here.";

        if (room.roomType == RoomType.Shop)
            text += "\n\nSupplies and upgrades may be available.";

        return text;
    }
}