import { Box } from '@mui/material';
import { FC } from 'react';

const Staging: FC = () => {
	return (
    <Box
      component='div'
      style={{
        position: 'absolute',
        top: 0,
        left: 0,
        right: 0
      }}
    >
			Staging
		</Box>
	);
}

export default Staging;