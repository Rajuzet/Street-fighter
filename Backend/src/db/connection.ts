import { Pool } from 'pg';
import { config } from '../config';

export const pool = new Pool({
  connectionString: config.databaseUrl,
});

export async function initializeDatabase() {
  try {
    await pool.connect();
    console.log('Connected to Postgres database');
  } catch (error) {
    console.error('Database connection failed', error);
    process.exit(1);
  }
}
