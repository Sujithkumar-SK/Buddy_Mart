pipeline {
    agent any

    environment {
        // 🔗 Your actual repository
        REPO_URL = 'https://github.com/Sujithkumar-SK/Buddy_Mart.git'
        
        // 🔐 Docker Hub credentials
        DOCKERHUB_CREDENTIALS = 'b1e5d709-e852-49b8-a76f-c5d5a164a31d'
        
        // 🐳 Docker Hub images
        FRONTEND_IMAGE = 'santhoshb7/ecommerce-frontend'
        BACKEND_IMAGE = 'santhoshb7/ecommerce-backend'
        
        // 🌐 Network
        DOCKER_NETWORK = 'ecommerce-network'
    }

    stages {
        stage('Checkout Code') {
            steps {
                echo "🔄 Checking out code..."
                git branch: 'main', url: "${REPO_URL}"
            }
        }

        stage('Set Image Tag') {
            steps {
                script {
                    def commitHash = sh(script: "git rev-parse --short HEAD", returnStdout: true).trim()
                    env.IMAGE_TAG = commitHash
                    echo "🏷️ Image tag: ${env.IMAGE_TAG}"
                }
            }
        }

        stage('Build Images') {
            parallel {
                stage('Build Frontend') {
                    steps {
                        dir('Frontend/ecommerce') {
                            sh '''
                                echo "🚀 Building frontend..."
                                docker build -t $FRONTEND_IMAGE:$IMAGE_TAG -f DockerFile .
                                docker tag $FRONTEND_IMAGE:$IMAGE_TAG $FRONTEND_IMAGE:latest
                            '''
                        }
                    }
                }
                stage('Build Backend') {
                    steps {
                        dir('Backend') {
                            sh '''
                                echo "🚀 Building backend..."
                                docker build -t $BACKEND_IMAGE:$IMAGE_TAG -f DockerFile .
                                docker tag $BACKEND_IMAGE:$IMAGE_TAG $BACKEND_IMAGE:latest
                            '''
                        }
                    }
                }
            }
        }

        stage('Push to Docker Hub') {
            steps {
                withCredentials([usernamePassword(credentialsId: "${DOCKERHUB_CREDENTIALS}", usernameVariable: 'DOCKER_USER', passwordVariable: 'DOCKER_PASS')]) {
                    sh '''
                        echo "🔐 Logging in to Docker Hub..."
                        echo $DOCKER_PASS | docker login -u $DOCKER_USER --password-stdin
                        
                        echo "📦 Pushing images..."
                        docker push $FRONTEND_IMAGE:$IMAGE_TAG
                        docker push $FRONTEND_IMAGE:latest
                        docker push $BACKEND_IMAGE:$IMAGE_TAG
                        docker push $BACKEND_IMAGE:latest
                    '''
                }
            }
        }

        stage('Deploy Application') {
            steps {
                sh '''
                    echo "🚀 Deploying application..."
                    
                    # Stop existing containers
                    docker-compose down || true
                    
                    # Create network if not exists
                    docker network create $DOCKER_NETWORK || true
                    
                    # Pull latest images
                    docker pull $FRONTEND_IMAGE:latest
                    docker pull $BACKEND_IMAGE:latest
                    
                    # Start application
                    docker-compose up -d
                    
                    # Wait for containers to start
                    sleep 30
                    
                    # Check container status
                    docker-compose ps
                '''
            }
        }

        stage('Health Check') {
            steps {
                sh '''
                    echo "🏥 Running health checks..."
                    
                    # Check backend health
                    curl -f http://localhost:5108/health || echo "Backend health check failed"
                    
                    # Check frontend health
                    curl -f http://localhost/health || echo "Frontend health check failed"
                    
                    # Show container logs
                    docker-compose logs --tail=50
                '''
            }
        }
    }

    post {
        always {
            sh 'docker logout'
        }
        success {
            echo "✅ Deployment successful!"
        }
        failure {
            echo "❌ Deployment failed!"
            sh 'docker-compose logs'
        }
    }
}