import { Vector3 } from '@react-three/fiber';
import { FC } from 'react';
import * as THREE from 'three'; 

const Placeholder: FC<{ num: number, position: Vector3 }> = ({ num, position }) => {
  return (
    <mesh 
      position={position}
      rotation={[Math.PI * -0.5, 0, 0]}
    >
      <circleGeometry args={[5 / 8 / 6 - 0.05, 16]} />
      <meshBasicMaterial side={THREE.DoubleSide} color={new THREE.Color(num === 1 ? 'white' : 'black')} />
    </mesh>
  );
}

export default Placeholder;