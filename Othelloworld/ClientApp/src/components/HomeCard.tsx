import { Button, Card, CardActions, CardContent, CardMedia, Typography } from '@mui/material';
import { FC } from 'react';

const HomeCard: FC<{ title: string, image: string, content: string, timestamp: string }> = ({ title, image, content, timestamp }) => {

	return (
    <Card
      sx={{
        maxWidth: 345,
        border: '1px solid #ffffff17'
      }}
    >
      <CardMedia
        sx={{
          backgroundColor: '#f06292',
          borderBottom: '1px solid #373737',
          color: '#121212',
          fontSize: 42,
          fontWeight: 900,
          lineHeight: '180px',
          textAlign: 'center'
        }}
        component="img"
        height="180"
        alt={ image }
        src={ image }
      />
      <CardContent>
        <Typography gutterBottom variant="h5" component="div">
          { title }
        </Typography>
        <Typography gutterBottom variant="caption" color="text.secondary" component="div">
          { new Date(timestamp).toUTCString() }
        </Typography>
        <Typography variant="body2">
          { content }
        </Typography>
      </CardContent>
      <CardActions>
        <Button size="small">Learn More</Button>
      </CardActions>
    </Card>
	);
}

export default HomeCard;