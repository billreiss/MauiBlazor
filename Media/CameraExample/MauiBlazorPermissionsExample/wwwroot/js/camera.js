window.startCameraFeed = async (video) => {
    try {
        const mediaStream = await navigator.mediaDevices.getUserMedia({
            video: true,
            //audio: true, // Uncommenting will request microphone permisssions. Be warned: this may cause audio feedback.
        });

        video.srcObject = mediaStream;

        await video.play();

        return null;
    } catch (err) {
        return err.message;
    }
};

window.getFrame = async (src, dest, dotNetHelper) => {
    let video = document.getElementById(src);
    let canvas = document.getElementById(dest);
    canvas.getContext('2d').drawImage(video, 0, 0, 320, 240);

    let dataUrl = canvas.toDataURL("image/jpeg");
    return dataUrl;
}

window.stopCameraFeed = (video) => {
    const stream = video.srcObject;

    if (!stream) {
        return;
    }

    const tracks = stream.getTracks();

    tracks.forEach(function (track) {
        track.stop();
    });

    video.srcObject = null;
};
