using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [Header("Pengaturan Halaman Menu")]
    public string Halaman_Menu;
    public string[] Halaman_Permainan;
    public string Halaman_Hasil;


    [Header("Pengaturan Halaman Soal")]

    public GameObject grupTempatJawaban;
    public GameObject grupButtonJawaban;

    public GameObject prefabsTempatJawaban;
    public GameObject prefabsButtonJawaban;

    public string kata;
    public int bobot;
    public string halamanSelanjutnya;


    public AudioClip suaraBenar;
    public AudioClip suaraSalah;
    public AudioSource suara;

    int step;
    string text_jawaban;

    [Header("Untuk Keperluan Debugging")]
    public int nilai;

    [Header("Pengaturan Halaman Hasil")]
    public GameObject[] bintang;
    public Text text_score;
    public int batas_bintang_1;
    public int batas_bintang_2;
    public int batas_bintang_3;



    // Start is called before the first frame update
    void Start()
    {
        nilai = PlayerPrefs.GetInt("nilai");

        if (SceneManager.GetActiveScene().name == Halaman_Menu)
        {
            for(int i = 0; i < Halaman_Permainan.Length; i++)
            {
                PlayerPrefs.SetString("halaman_permainan_"+i, Halaman_Permainan[i]);
            }
            PlayerPrefs.SetInt("total_halaman_permainan", Halaman_Permainan.Length);
            PlayerPrefs.SetString("halaman_hasil", Halaman_Hasil);
        }
        else if (SceneManager.GetActiveScene().name == PlayerPrefs.GetString("halaman_hasil"))
        {
            PemberianBintang();
        }

        for(int i = 0; i < PlayerPrefs.GetInt("total_halaman_permainan"); i++)
        {
            if(SceneManager.GetActiveScene().name == PlayerPrefs.GetString("halaman_permainan_" + i))
            {
                if (SceneManager.GetActiveScene().name == PlayerPrefs.GetString("halaman_permainan_0"))
                {
                    PlayerPrefs.SetInt("nilai", 0);
                    nilai = PlayerPrefs.GetInt("nilai");
                    GenerateAll();
                }
                else
                {
                    GenerateAll();
                }
            }
        }
    }

    void GenerateAll()
    {
        step = 0;
        for (int i = 0; i < kata.Length; i++)
        {
            GenerateTempatJawaban(i);
            GenerateButtonJawaban(i);
        }
        RandomPosisiDariButtonJawaban();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GenerateTempatJawaban(int i)
    {
        var tempatJawaban = Instantiate(prefabsTempatJawaban, grupTempatJawaban.transform.position, grupTempatJawaban.transform.rotation);
        tempatJawaban.transform.SetParent(grupTempatJawaban.transform);
        tempatJawaban.name = kata.Substring(i, 1).ToUpper();
    }

    void GenerateButtonJawaban(int i)
    {
        var buttonJawaban = Instantiate(prefabsButtonJawaban, grupButtonJawaban.transform.position, grupButtonJawaban.transform.rotation);
        buttonJawaban.transform.SetParent(grupButtonJawaban.transform);
        buttonJawaban.transform.GetChild(0).GetComponent<Text>().text = kata.Substring(i, 1).ToUpper();
        buttonJawaban.name = kata.Substring(i, 1).ToUpper();
        buttonJawaban.GetComponent<Button>().onClick.AddListener(delegate { ButtonJawabanOnClick(buttonJawaban); });
    }

    void ButtonJawabanOnClick(GameObject btn)
    {
        string text = btn.transform.GetChild(0).GetComponent<Text>().text;
        btn.SetActive(false);
        SimpanJawabanUserKeGrupJawaban(text);
        CekJawabanUser();
    }

    void SimpanJawabanUserKeGrupJawaban(string text)
    {
        GameObject child = grupTempatJawaban.transform.GetChild(step).gameObject;
        Text child_text = child.transform.GetChild(0).GetComponent<Text>();
        child_text.text = text;
        text_jawaban = text_jawaban + child_text.text;
        step++;
    }

    void CekJawabanUser()
    {
        if (step == kata.Length)
        {
            if (text_jawaban == kata.ToUpper())
            {
                Debug.Log("Jawaban Benar");
                suara.clip = suaraBenar;
                suara.Play();
                TambahNilai();
            }
            else
            {
                Debug.Log("Jawaban salah");
                suara.clip = suaraSalah;
                suara.Play();
            }
            StartCoroutine(nextHalaman());
        }
    }

    void TambahNilai()
    {
        
        nilai = nilai + bobot;
        PlayerPrefs.SetInt("nilai", nilai);
    }

    IEnumerator nextHalaman()
    {
        yield return new WaitForSeconds(1f);
        PindahHalaman(halamanSelanjutnya);
    }

    void RandomPosisiDariButtonJawaban()
    {
        List<int> ints = new List<int>();
        List<int> values = new List<int>();

        for (int i = 0; i < kata.Length; ++i)
        {
            ints.Add(i);
        }

        for (int i = 0; i < kata.Length; ++i)
        {
            int index = Random.Range(0, ints.Count);
            values.Add(ints[index]);
            ints.RemoveAt(index);
        }

        for (int i = 0; i < kata.Length; ++i)
        {
            foreach(Transform child in grupButtonJawaban.transform)
            {
                child.SetSiblingIndex(values[i]);
            }
        }
    }

    public void PindahHalaman(string halamanTujuan)
    {
        SceneManager.LoadScene(halamanTujuan);
    }

    public void Open_Popup(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    public void Close_Popup(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    public void Keluar_Aplikasi()
    {
        Application.Quit();
    }

    void PemberianBintang()
    {
        if (nilai <= batas_bintang_1)
        {
            bintang[0].SetActive(true);
            bintang[1].SetActive(false);
            bintang[2].SetActive(false);
        }
        else if (nilai <= batas_bintang_2)
        {
            bintang[0].SetActive(true);
            bintang[1].SetActive(true);
            bintang[2].SetActive(false);
        }
        else if (nilai <= batas_bintang_3)
        {
            bintang[0].SetActive(true);
            bintang[1].SetActive(true);
            bintang[2].SetActive(true);
        }

        text_score.text = "Nilai: " + nilai;
    }

}
