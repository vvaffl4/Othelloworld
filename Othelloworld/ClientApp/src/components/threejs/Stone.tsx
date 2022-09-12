import { Vector3 } from '@react-three/fiber';
import { FC } from 'react';
import * as THREE from 'three'; 

const Stone: FC<{ num: number, position: Vector3 }> = ({ num, position }) => {
  return (
    <mesh 
      position={position}
      scale={[1, 0.4, 1]}
    >
      <octahedronGeometry args={[5 / 8 / 2 - 0.05, 3]} />
      <meshBasicMaterial color={new THREE.Color(num === 1 ? 'white' : 'black')}/>
    </mesh>
  );
}

export default Stone;