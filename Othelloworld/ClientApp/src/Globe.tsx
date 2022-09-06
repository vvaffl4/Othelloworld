import { useFrame, useLoader } from '@react-three/fiber';
import { FC, Suspense, useRef } from 'react';
import * as THREE from 'three';
import { Mesh } from 'three';

const Globe: FC = () => {
  const ref = useRef<Mesh>(null!);

  const base = useLoader(THREE.TextureLoader, "/img/worldColour.5400x2700.jpg");
  const displacement = useLoader(THREE.TextureLoader, "/img/srtm_ramp2.world.5400x2700.jpg")

  useFrame(() => {
    // ref.current.rotation.x += 0.0005;
    ref.current.rotation.y += 0.001;
  })

  return (
    <Suspense fallback={null}>
      <mesh
        ref={ref}
        castShadow
        receiveShadow
      >
        <sphereGeometry args={[1, 720, 360]}/>
        <meshStandardMaterial
          map={base}
          displacementMap={displacement}
          displacementScale={0.05}
        />
      </mesh>
    </Suspense>
  );
}

export default Globe;