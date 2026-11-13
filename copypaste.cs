using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

class Program
{
    static readonly Color Bg = Color.FromArgb(15, 18, 32);
    static readonly Color Card = Color.FromArgb(24, 28, 48);
    static readonly Color Line = Color.FromArgb(42, 48, 80);
    static readonly Color Txt = Color.FromArgb(232, 235, 247);
    static readonly Color Muted = Color.FromArgb(125, 134, 169);
    static readonly Color Accent = Color.FromArgb(109, 124, 255);
    static readonly Color Ok = Color.FromArgb(46, 204, 143);

    static string DataFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "copypaste", "frasi.txt");

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        var f = new Form
        {
            Text = "copypaste",
            ClientSize = new Size(760, 740),
            MinimumSize = new Size(480, 400),
            BackColor = Bg,
            StartPosition = FormStartPosition.CenterScreen,
            Font = new Font("Segoe UI", 10f)
        };

        var title = new Label
        {
            Text = "copypaste", ForeColor = Accent, AutoSize = true,
            Font = new Font("Segoe UI Semibold", 20f), Location = new Point(24, 14)
        };
        var sub = new Label
        {
            Text = "scrivi, clicca Copia, incolla", ForeColor = Muted, AutoSize = true,
            Location = new Point(28, 52)
        };
        var box = new Panel { Location = new Point(0, 82), Size = new Size(f.ClientSize.Width, f.ClientSize.Height - 82), AutoScroll = true, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
        f.Controls.AddRange(new Control[] { title, sub, box });

        var tbs = new TextBox[10];
        var saved = Load();
        for (int i = 0; i < 10; i++)
        {
            var row = new Panel { BackColor = Card, Location = new Point(20, i * 68), Size = new Size(box.ClientSize.Width - 40 - SystemInformation.VerticalScrollBarWidth, 60), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            var num = new Label { Text = (i + 1).ToString(), ForeColor = Muted, Location = new Point(10, 18), Size = new Size(26, 24), TextAlign = ContentAlignment.MiddleCenter };
            var tb = new TextBox
            {
                Multiline = true, BorderStyle = BorderStyle.None, BackColor = Card, ForeColor = Txt,
                ScrollBars = ScrollBars.Vertical, Location = new Point(44, 8), Size = new Size(row.Width - 44 - 120, 44),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Text = saved[i]
            };
            var btn = new Button
            {
                Text = "Copia", FlatStyle = FlatStyle.Flat, BackColor = Accent, ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 10f), Cursor = Cursors.Hand,
                Location = new Point(row.Width - 104, 10), Size = new Size(94, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btn.FlatAppearance.BorderSize = 0;
            var tm = new Timer { Interval = 1000 };
            tm.Tick += (s, e) => { tm.Stop(); btn.Text = "Copia"; btn.BackColor = Accent; };
            var tbl = tb;
            btn.Click += (s, e) =>
            {
                if (tbl.Text.Length == 0) return;
                try { Clipboard.SetText(tbl.Text); } catch { return; }
                btn.Text = "Copiato ✓"; btn.BackColor = Ok;
                tm.Stop(); tm.Start();
            };
            tb.TextChanged += (s, e) => Save(tbs);
            tbs[i] = tb;
            row.Controls.AddRange(new Control[] { num, tb, btn });
            box.Controls.Add(row);
        }
        Application.Run(f);
    }

    static string[] Load()
    {
        var r = new string[10];
        for (int i = 0; i < 10; i++) r[i] = "";
        try
        {
            if (!File.Exists(DataFile)) return r;
            var lines = File.ReadAllLines(DataFile);
            for (int i = 0; i < 10 && i < lines.Length; i++)
                r[i] = Encoding.UTF8.GetString(Convert.FromBase64String(lines[i]));
        }
        catch { }
        return r;
    }

    static void Save(TextBox[] tbs)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DataFile));
            var lines = new string[10];
            for (int i = 0; i < 10; i++)
                lines[i] = tbs[i] == null ? "" : Convert.ToBase64String(Encoding.UTF8.GetBytes(tbs[i].Text));
            File.WriteAllLines(DataFile, lines);
        }
        catch { }
    }
}
