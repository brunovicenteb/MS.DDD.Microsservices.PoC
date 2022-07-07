FROM node:alpine    
EXPOSE 3000
COPY Benefit.Consumer.Worker.Test/Mock/db.json /opt/mock.json
RUN yarn global add json-server
CMD ["json-server", "-H", "0.0.0.0", "/opt/mock.json"]