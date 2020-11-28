using UnityEngine;

[System.Serializable]
public class Dialogue {

    public Color leftProfileColor = Color.red;
    public Color rightProfileColor = Color.white;
    public Color noProfileColor = new Color(0.75f, 0.75f, 0.75f);
    public float defaultFontSize = 30;
    public float defaultSpeed = 0.05f;
    public SubDialogue[] subDialogues;

    [System.Serializable]
    public class SubDialogue {
        public ProfileLocation profileLocation;
        public int fontSize;
        public float textSpeed;
        [TextArea(3, 10)]
        public string text;
    }

    [System.Serializable]
    public enum ProfileLocation {
        Left,
        Right,
        None
    }
}
