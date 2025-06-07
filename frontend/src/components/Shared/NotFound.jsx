import { Button, Typography, Box } from '@mui/material';
import { Link } from 'react-router-dom';

export default function NotFound() {
    return (
        <Box sx={{ textAlign: 'center', mt: 10 }}>
            <Typography variant="h3">404 - Page Not Found</Typography>
            <Button component={Link} to="/" variant="contained" sx={{ mt: 3 }}>
                Go Home
            </Button>
        </Box>
    );
}