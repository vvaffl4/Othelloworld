﻿import { useRef, FC, useState, useEffect } from "react";
import { ControlledCameras } from "./ControlledCamera";
import CameraControls from "camera-controls";
import { useAppSelector } from "../../store/Hooks";

const Camera: FC = () => {
  const controls = useRef(null!);
  const mode = useAppSelector(state => state.game.controls.mode);

  //const [mode, setMode] = useState<"perspective" | "orthographic">(
  //  "perspective"
  //);

  //useEffect(() => {
  //  const changeView = (e) => {
  //    if (e.keyCode !== 32) return;

  //    setMode((state) =>
  //      state === "perspective"
  //        ? "orthographic"
  //        : "perspective"
  //    );
  //  };

  //  window.addEventListener("keyup", changeView);
  //  return () => window.removeEventListener("keyup", changeView);
  //}, []);

  // const logEvent = (e) => console.log(e.type);

  return (
    <ControlledCameras
      // The ref contains all the normal CameraControls  methods like setLookAt, zoomTo, etc. etc.
      // See the camera controls docs here: https://github.com/yomotsu/camera-controls
      ref={controls}
      mode={mode} // orthographic or perspective
      /* ******* */
      /* CONTROLS */
      /* ******* */
      maxPolarAngle={Math.PI / 2}
      restThreshold={0.01}
      dampingFactor={0.05}
      /* ******* */
      /* CAMERAS */
      /* ******* */
      // You can change these to jump to a new position
      // If you change the prop for the inactive mode, the camera will move
      // there on the next transition change
      perspectivePosition={[0, 5, 10]}
      perspectiveTarget={[0, 0, 0]}
      orthographicPosition={[0, 2, 0]}
      orthographicTarget={[0, 0, 0]}
      // Set the *initial* camera params for both modes
      // Currently, changing these is not well supported (weird stuff will happen)
      perspectiveCameraProps={{
        fov: 50,
        near: 1,
        far: 100
      }}
      orthographicCameraProps={{
        zoom: 100, // Warning: changing the ortho zoom prop after initialization is not yet supported
        near: -100,
        far: 100
      }}
      /* ******* */
      /* EVENTS  */
      /* ******* */
      // onControlStart={logEvent}
      // onControl={(logEvent}
      // onControlEnd={(logEvent}
      // onTransitionStart={logEvent}
      // onUpdate={logEvent}
      // onWake={logEvent}
      // Note: onRest will fire when the controls *appear* to have stopped.
      // You can fine tune this with restThreshold
      //onRest={onRest}
      // Sleep will fire some time later when when all damped movement has been applied
      // onSleep={logEvent}

      /* ******* */
      /* INPUT   */
      /* ******* */
      mouseButtons={{
        left:
          mode === "perspective"
            ? CameraControls.ACTION.ROTATE
            : CameraControls.ACTION.TRUCK,
        middle:
          mode === "perspective"
            ? CameraControls.ACTION.DOLLY
            : CameraControls.ACTION.ZOOM,
        //right: CameraControls.ACTION.TRUCK,
        wheel:
          mode === "perspective"
            ? CameraControls.ACTION.DOLLY
            : CameraControls.ACTION.ZOOM
        // shiftLeft: CameraControls.ACTION.NONE,
      }}
      touches={{
        one: CameraControls.ACTION.TOUCH_ROTATE,
        two:
          mode === "perspective"
            ? CameraControls.ACTION.TOUCH_DOLLY_TRUCK
            : CameraControls.ACTION.TOUCH_ZOOM_TRUCK
        // three:
        //   mode === "perspective"
        //     ? CameraControls.ACTION.TOUCH_DOLLY_TRUCK
        //     : CameraControls.ACTION.TOUCH_ZOOM_TRUCK
      }}
    />
  );
}

export default Camera;