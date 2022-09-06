import { Container, Paper, Typography } from '@mui/material';
import { FC } from 'react';

const Home: FC = () => {
	return (
		 <Container>
      <Typography
        variant="h1"
        color="white"
        sx={{
          padding: '100px 200px 0 0',
          fontWeight: 700,
          textAlign: 'right'
        }}
      >
        OTHELLO
      </Typography>
      <Paper sx={{
        mt: 10,
        overflow: 'hidden'
      }}>
        <div
          style={{ padding: '20px' }}
        >
          <Typography
            variant="h2"
          >
            Home
          </Typography>
        </div>
      </Paper>
    </Container>
	)
}

export default Home;