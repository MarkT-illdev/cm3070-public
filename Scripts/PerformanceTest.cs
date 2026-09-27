// Unity Performance Test
// FPS, Main Thread, Render Thread, Batches
// GPU exlcuded, not measurable at MacBook Pro
using UnityEngine;
using Unity.Profiling;
using UnityEngine.Playables;

public class PerformanceTest : MonoBehaviour
{
    public PlayableDirector director;

    ProfilerRecorder mainThread;
    ProfilerRecorder renderThread;
    ProfilerRecorder batches;

    float totalTime;
    double totalMainMs;
    double totalRenderMs;
    long totalBatches;
    int frames;

    void OnEnable()
    {
        director = GetComponent<PlayableDirector>();
        
        mainThread = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "CPU Main Thread Frame Time");
        renderThread = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "CPU Render Thread Frame Time");
        batches = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count");
    }

    void Update()
    {
        if (director != null && director.state != PlayState.Playing)
            return;

        totalTime += Time.unscaledDeltaTime;
        totalMainMs += mainThread.LastValue * 1e-6;
        totalRenderMs += renderThread.LastValue * 1e-6;
        totalBatches += batches.LastValue;
        frames++;
    }

    void OnDisable()
    {
        if (frames > 0)
        {
            Debug.Log(
                $"Average FPS: {frames / totalTime:F1}\n" +
                $"Average Main Thread; {totalMainMs / frames:F2} ms\n" +
                $"Average Render Thread; {totalRenderMs / frames:F2} ms\n" +
                $"Average Batches: {(double)totalBatches / frames:F1}"
            );
        }

        mainThread.Dispose();
        renderThread.Dispose();
        batches.Dispose();
    }
}
