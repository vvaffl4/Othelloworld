import { Button, Snackbar } from '@mui/material';
import { FC, useState } from 'react';

const ErrorMessage: FC = () => {
  const [open, setOpen] = useState(true);

  const handleClose = () => {
    console.log('Closed');
    setOpen(false);
  }
	return (
    <Snackbar
      open={open}
      autoHideDuration={6000}
      message="You've received an error. Oh no!"
      color="error"
      action={
        <Button
          size="small"
          color="error"
          onClick={handleClose}
        >
           Close
        </Button>
      }
      sx={{ bottom: 90 }}
    />
		);
}

export default ErrorMessage;