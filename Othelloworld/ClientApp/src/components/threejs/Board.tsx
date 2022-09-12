import { ThreeEvent, Vector3 } from '@react-three/fiber';
import React, { FC, useCallback, useEffect, useRef, useState } from 'react';
import { shallowEqual } from 'react-redux';
import * as THREE from 'three';
import Placeholder from './Placeholder';
import Stone from './Stone';
import { putStone } from '../../store/Game';
import { useAppDispatch, useAppSelector } from '../../store/Hooks';

const Board: FC<{position: Vector3 }> = (props) => {
  const boardSpaces = 8;
  const boardSize = 5;
  const spaceSize = boardSize / boardSpaces;    

  const ref = useRef<THREE.PlaneGeometry>(null!);
  const [turn, setTurn] = useState<number>(1);

  const dispatch = useAppDispatch();
  const game = useAppSelector(state => state.game, shallowEqual);
 
  useEffect(() => {
    for (let i = 0; i < boardSpaces * boardSpaces; i++) {
        ref.current!.addGroup(
          i * 6,
          6, 
          ((i + Math.floor(i / boardSpaces)) % 2));
    }
  }, [ref]);

  const handleClick = useCallback((event: ThreeEvent<MouseEvent>) => {
    if (event.eventObject.name === 'board' && event.faceIndex !== undefined) {
      const index = Math.floor(event.faceIndex * 0.5);
      const xIndex = index % 8
      const yIndex = Math.floor(index / 8);

      dispatch(putStone([xIndex, yIndex])); 
    }
  }, [game.board]);

  return (
    <>
      <mesh
        name='board'
        material={[
          new THREE.MeshBasicMaterial({ color: new THREE.Color('green') }),
          new THREE.MeshBasicMaterial({ color: new THREE.Color('darkgreen') })
        ]}
        rotation={new THREE.Euler(-90 * (Math.PI / 180), 0, 0)}
        onClick={handleClick}
      >
        <planeGeometry 
          ref={ref}
          args={[5, 5, 8, 8]}
        />
      </mesh>
      {game.board
        .map((row, yIndex) =>
          row.map((cell, xIndex) => {
            const index = yIndex * 8 + xIndex;

            return cell !== 0 && (
              <Stone
                key={index}
                num={cell}
                position={[
                  -(spaceSize * ((boardSpaces - 1) * 0.5)) + xIndex * spaceSize,
                  0.1,
                  -(spaceSize * ((boardSpaces - 1) * 0.5)) + yIndex * spaceSize
                ]}
              />
            )
          }))}
      {game.placeholders.map(([ posX, posY ], index) => (
        <Placeholder
          key={index}
          num={game.turn!}
          position={[
            -(spaceSize * ((boardSpaces - 1) * 0.5)) + posX * spaceSize,
            0.01,
            -(spaceSize * ((boardSpaces - 1) * 0.5)) + posY * spaceSize
          ]}
        />
      ))}
    </>
  )
}

export default Board;