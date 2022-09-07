import { FC } from 'react';
import { Outlet } from 'react-router-dom';

const HomeContainer: FC = () => {
  return (
    <div
      style={{
        position: 'absolute',
        top: 100,
        left: 0,
        right: 0
      }}
    >
      <Outlet />
    </div>
  )
}

export default HomeContainer;