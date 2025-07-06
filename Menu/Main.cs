using Locomotion;
using Photon.Pun;
using Photon.Voice.Unity;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using static Locomotion.Player;

namespace Cappuchino.Menu
{
    public class Main : MonoBehaviour
    {
        public static float startX = -1f;
        public static float startY = -1f;

        public static float subThingy;
        public static float subThingyZ;

        public static void WASDFly()
        {
            Player.Instance.playerRigidbody.velocity = new Vector3(0f, 0.067f, 0f);

            bool W = Input.GetKey(KeyCode.W);
            bool A = Input.GetKey(KeyCode.A);
            bool S = Input.GetKey(KeyCode.S);
            bool D = Input.GetKey(KeyCode.D);
            bool Space = Input.GetKey(KeyCode.Space);
            bool Ctrl = Input.GetKey(KeyCode.LeftControl);

            if (Input.GetMouseButton(1))
            {
                Transform parentTransform = Player.Instance.transform;
                Quaternion currentRotation = parentTransform.rotation;
                Vector3 euler = currentRotation.eulerAngles;

                if (startX < 0)
                {
                    startX = euler.y;
                    subThingy = Input.mousePosition.x / Screen.width;
                }
                if (startY < 0)
                {
                    startY = euler.x;
                    subThingyZ = Input.mousePosition.y / Screen.height;
                }

                float newX = startY - ((((Input.mousePosition.y / Screen.height) - subThingyZ) * 360) * 1.33f);
                float newY = startX + ((((Input.mousePosition.x / Screen.width) - subThingy) * 360) * 1.33f);

                newX = (newX > 180f) ? newX - 360f : newX;
                newX = Mathf.Clamp(newX, -90f, 90f);

                parentTransform.rotation = Quaternion.Euler(newX, newY, euler.z);
            }
            else
            {
                startX = -1;
                startY = -1;
            }

            float speed = 10f;
            if (Input.GetKey(KeyCode.LeftShift))
                speed *= 2f;

            if (W)
                Player.Instance.transform.position += Player.Instance.transform.forward * Time.deltaTime * speed;

            if (S)
                Player.Instance.transform.position += Player.Instance.transform.forward * Time.deltaTime * -speed;

            if (A)
                Player.Instance.transform.position += Player.Instance.transform.right * Time.deltaTime * -speed;

            if (D)
                Player.Instance.transform.position += Player.Instance.transform.right * Time.deltaTime * speed;

            if (Space)
                Player.Instance.transform.position += new Vector3(0f, Time.deltaTime * speed, 0f);

            if (Ctrl)
                Player.Instance.transform.position += new Vector3(0f, Time.deltaTime * -speed, 0f);
        }

        public static void PCButtons()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out var Ray, 512f);

                Find("Global/CapuchinPhysRig/XR Origin/CapuchinPlayer/Main Camera/CameraChild/PlayerModel/CapuchinRemadeMale/Capuchin/torso/shoulder.L/upperarm.L/lowerarm.L/hand.L/index.L/index.002.L/index.001.L")
                    .transform.position = Ray.point;
            } else
            {
                Find("Global/CapuchinPhysRig/XR Origin/CapuchinPlayer/Main Camera/CameraChild/PlayerModel/CapuchinRemadeMale/Capuchin/torso/shoulder.L/upperarm.L/lowerarm.L/hand.L/index.L/index.002.L/index.001.L")
                    .transform.position = Player.Instance.transform.position;
            }
        }

        private static bool wasdfly;
        private static bool fly;
        private static bool buttonpc;
        private static bool rgb;
        private static bool namecycle;
        private static new string name = "goldentrophy";
        private static string song = "audio.wav";
        private static string room = "1";
        public void OnGUI()
        {
            wasdfly = GUI.Toggle(new Rect(10, 10, 200, 20), wasdfly, "WASD Fly");
            fly = GUI.Toggle(new Rect(210, 10, 200, 20), fly, "Fly VR");
            buttonpc = GUI.Toggle(new Rect(10, 30, 200, 20), buttonpc, "PC Buttons");
            rgb = GUI.Toggle(new Rect(10, 50, 200, 20), rgb, "RGB");
            namecycle = GUI.Toggle(new Rect(210, 50, 200, 20), namecycle, "Namecycle");
            platforms = GUI.Toggle(new Rect(10, 190, 200, 20), platforms, "Platforms");
            name = GUI.TextArea(new Rect(210, 70, 200, 20), name);
            song = GUI.TextArea(new Rect(210, 100, 200, 20), song);
            room = GUI.TextArea(new Rect(210, 130, 200, 20), room);

            if (GUI.Button(new Rect(10, 70, 200, 20), "Setname"))
            {
                FusionPlayer.Instance.name = name;
                FusionPlayer.Instance.SetUsername(name);
                FusionPlayer.Instance.UpdateUsername(name);
            }

            if (GUI.Button(new Rect(10, 100, 200, 20), "Sound"))
            {
                Recorder Mic = ClientMicDetection.instance.Recorder;

                Mic.DebugEchoMode = true;
                Mic.SourceType = Recorder.InputSourceType.AudioClip;
                Mic.AudioClip = LoadSoundFromFile(song);
                Mic.RestartRecording();
            }

            if (GUI.Button(new Rect(500, 100, 200, 20), "Fixsound"))
            {
                Recorder Mic = ClientMicDetection.instance.Recorder;

                Mic.DebugEchoMode = true;
                Mic.SourceType = Recorder.InputSourceType.Microphone;
                Mic.AudioClip = LoadSoundFromFile(song);
                Mic.RestartRecording();
            }

            if (GUI.Button(new Rect(10, 130, 200, 20), "Joinroom"))
                FusionHub.JoinOrCreateRoom(new FusionHub.LobbyParams { Name = room, MaxPlayers = 10, IsOpen = false });

            if (GUI.Button(new Rect(10, 160, 200, 20), "Leaveroom"))
                FusionHub.Leave();
        }

        public static AudioClip LoadSoundFromFile(string file)
        {
            file = Path.Combine(Application.dataPath, file);
            Debug.Log(file);

            WWW www = new WWW("file://" + file);
            while (!www.isDone) { }

            AudioClip clip = www.GetAudioClip();
            if (clip != null)
            {
                clip.name = Path.GetFileName(file);
                return clip;
            }
            else
                Debug.LogError($"Failed to load: {file}");

            return null;
        }

        private static float colordelay;
        private static float namecycledelay;
        private static bool type;
        private static bool platforms;
        private static GameObject l;
        private static GameObject r;
        public void Update()
        {
            if (wasdfly)
                WASDFly();

            if (buttonpc)
                PCButtons();

            if (rgb)
            {
                if (Time.time > colordelay)
                {
                    colordelay = Time.time + 0.1f;
                    float h = (Time.frameCount / 180f) % 1f;
                    FusionPlayer.Instance.SetColor(Color.HSVToRGB(h, 1f, 1f));
                    FusionPlayer.Instance.SetColorNew(Color.HSVToRGB(h, 1f, 1f));
                    FusionPlayer.Instance.__Color = Color.HSVToRGB(h, 1f, 1f);
                    FusionPlayer.Instance.___Color = Color.HSVToRGB(h, 1f, 1f);
                    FusionPlayer.Instance.skin.material.color = Color.HSVToRGB(h, 1f, 1f);
                }
            }

            if (namecycle)
            {
                if (Time.time > namecycledelay)
                {
                    namecycledelay = Time.time + 1f;
                    type = !type;
                    string sname = type ? ".gg iidk" : "goldentrophy";
                    FusionPlayer.Instance.name = sname;
                    FusionPlayer.Instance.Username = sname;
                    FusionPlayer.Instance.SetUsername(sname);
                    FusionPlayer.Instance.UpdateUsername(sname);
                }
            }

            if (platforms)
            {
                if (leftGrip)
                {
                    if (l == null)
                    {
                        l = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        l.transform.position = Player.Instance.LeftHand.transform.position;
                        l.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    }
                } else
                {
                    if (l != null)
                        Object.Destroy(l);
                }

                if (rightGrip)
                {
                    if (r == null)
                    {
                        r = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        r.transform.position = Player.Instance.RightHand.transform.position;
                        r.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    }
                }
                else
                {
                    if (r != null)
                        Object.Destroy(r);
                }
            }
        }

        public void Start()
        {
            SceneManager.LoadScene("CapuchinCopy");

            GameObject.Find("Global/Managment/Hallo :D").SetActive(false); // anti cheat mechanism
        }

        public static Camera MainCamera
        {
            get
            {
                if (mainCamera == null)
                    mainCamera = Player.Instance.transform.Find("Main Camera").GetComponent<Camera>();

                return mainCamera;
            }
        }

        private static Camera mainCamera;

        private static Dictionary<string, GameObject> objs = new Dictionary<string, GameObject> { };
        public static GameObject Find(string find)
        {
            if (objs.TryGetValue(find, out GameObject go))
                return go;

            GameObject tgo = GameObject.Find(find);
            if (tgo != null)
                objs.Add(find, tgo);

            return tgo;
        }

        private static bool leftGrip
        {
            get => InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out bool outp) & outp;
        }

        private static bool rightGrip
        {
            get => InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out bool outp) & outp;
        }

        private static bool leftTrig
        {
            get => InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool outp) & outp;
        }

        private static bool rightTrig
        {
            get => InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool outp) & outp;
        }

        private static bool a
        {
            get => InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool outp) & outp;
        }

        private static bool b
        {
            get => InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out bool outp) & outp;
        }

        private static bool x
        {
            get => InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool outp) & outp;
        }

        private static bool y
        {
            get => InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out bool outp) & outp;
        }
    }
}
