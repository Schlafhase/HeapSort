using System.Diagnostics;
using System.Numerics;

namespace Graphical;

public static class AnimationRenderer
{
    public static void RenderToFile(
        this AnimatedGraphic g,
        string filepath,
        Action<Graphic, string> renderAndSave,
        int fps = 30
    )
    {
        string dirName = Guid.NewGuid().ToString();
        try
        {
            Graphic? currentFrame = g.Advance(1d / fps);

            Directory.CreateDirectory(dirName);
            for (int frameNo = 1; currentFrame is not null; frameNo++)
            {
                renderAndSave(
                    currentFrame,
                    $"{dirName}/frame_{frameNo.ToString().PadLeft(9, '0')}.jpg"
                );
                currentFrame = g.Advance(1d / fps);
            }

            if (Path.GetDirectoryName(filepath) is { } dir and not "")
            {
                Directory.CreateDirectory(dir);
            }

            ProcessStartInfo psi = new()
            {
                FileName = "ffmpeg",
                Arguments =
                    $@"-framerate {fps} -y -i {dirName}/frame_%09d.jpg -c:v libx264 -crf 1 -vf ""scale=iw*min(1920/iw\,1080/ih):ih*min(1920/iw\,1080/ih), pad=1920:1080:(1920-iw*min(1920/iw\,1080/ih))/2:(1080-ih*min(1920/iw\,1080/ih))/2"" -pix_fmt yuv420p {filepath}",
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            using Process? p =
                Process.Start(psi)
                ?? throw new InvalidOperationException("Failed to start ffmpeg. Is it installed?");

            string stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                throw new Exception($"ffmpeg failed: \n{stderr}");
            }
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            Directory.Delete(dirName, true);
        }
    }
}
